using System.Linq.Expressions;
using System.Text.Json;
using PPH.Library.Helpers;
using PPH.Library.Models;
using SQLite;

namespace PPH.Library.Services
{
    public class WordStorage : IWordStorage
    {
        public const int NumberOfWords = 7351;
        public const string DbName = "worddb.sqlite3";
        public static readonly string WordDbPath = PathHelper.GetLocalFilePath(DbName);

        private SQLiteAsyncConnection _connection;
        private SQLiteAsyncConnection Connection => _connection ??= new SQLiteAsyncConnection(WordDbPath);

        private readonly IPreferenceStorage _preferenceStorage;
        private readonly IAlertService _alertService;

        public WordStorage(IPreferenceStorage preferenceStorage, IAlertService alertService)
        {
            _preferenceStorage = preferenceStorage;
            _alertService = alertService;
        }

        public bool IsInitialized =>
            _preferenceStorage.Get(WordStorageConstant.VersionKey, default(int)) == WordStorageConstant.Version &&
            File.Exists(WordDbPath);

        public async Task InitializeAsyncForFirstTime()
        {
            await Connection.CreateTableAsync<ObjectWord>();

            using var httpClient = new HttpClient();
            var wordListResponse = await GetWordListAsync(httpClient);
            if (wordListResponse == null) return; // 如果获取单词列表失败，提前返回

            var baicizhanListResult = wordListResponse.Item1;
            var wordList = baicizhanListResult.List;

            // 执行数据修正
            CorrectWordList(wordList);

            var objectWords = new List<ObjectWord>();
            for (var i = 0; i < wordList.Length; i++)
            {
                var word = wordList[i];
                if (IsValidWord(word, i))
                {
                    var wordDetails = await GetWordDetailsAsync(httpClient, word);
                    if (wordDetails != null)
                    {
                        objectWords.Add(wordDetails);
                    }
                }

                // 每50个单词批量插入
                if ((i + 1) % 50 == 0)
                {
                    await InsertWordBatchAsync(objectWords);
                }
            }

            // 最后一次插入
            if (objectWords.Any())
            {
                await InsertWordBatchAsync(objectWords);
            }

            _preferenceStorage.Set(WordStorageConstant.VersionKey, WordStorageConstant.Version);
            await Connection.CloseAsync();
        }

        private async Task<Tuple<BaicizhanListResult, string>> GetWordListAsync(HttpClient httpClient)
        {
            try
            {
                var response = await httpClient.GetAsync("https://cdn.jsdelivr.net/gh/lyc8503/baicizhan-word-meaning-API/data/list.json");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<BaicizhanListResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return result != null ? Tuple.Create(result, json) : null;
            }
            catch (Exception e)
            {
                await _alertService.AlertAsync(ErrorMessageHelper.HttpClientErrorTitle, ErrorMessageHelper.GetHttpClientError("百词斩单词服务器", e.Message));
                return null;
            }
        }

        private void CorrectWordList(string[] wordList)
        {
            wordList[7769] = "may";
            wordList[7913] = "pacific";
            wordList[7916] = "Turkey";
        }

        private bool IsValidWord(string word, int index)
        {
            return !(word.Contains(' ') || word.Contains('/') || word.Contains('_') || index == 7609);
        }

        private async Task<ObjectWord> GetWordDetailsAsync(HttpClient httpClient, string word)
        {
            try
            {
                var wordDetailsJson = await httpClient.GetStringAsync($"https://cdn.jsdelivr.net/gh/lyc8503/baicizhan-word-meaning-API/data/words/{word}.json");
                return JsonSerializer.Deserialize<BaicizhanWordResult>(wordDetailsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) is BaicizhanWordResult wordResult
                    ? new ObjectWord
                    {
                        Word = wordResult.word,
                        Accent = wordResult.accent,
                        CnMeaning = wordResult.mean_cn,
                        EnMeaning = wordResult.mean_en,
                        Sentence = wordResult.sentence,
                        SentenceTrans = wordResult.sentence_trans,
                        Phrase = wordResult.sentence_phrase,
                        Etyma = wordResult.word_etyma
                    }
                    : null;
            }
            catch (Exception e)
            {
                await _alertService.AlertAsync(ErrorMessageHelper.JsonDeserializationErrorTitle, ErrorMessageHelper.GetJsonDeserializationError("百词斩单词服务器", e.Message));
                return null;
            }
        }

        private async Task InsertWordBatchAsync(List<ObjectWord> wordObjects)
        {
            if (wordObjects.Any())
            {
                await Connection.InsertAllAsync(wordObjects);
                wordObjects.Clear();
            }
        }

        public async Task InitializeAsync()
        {
            await using var dbFileStream = new FileStream(WordDbPath, FileMode.OpenOrCreate);
            await using var dbAssetStream = typeof(WordStorage).Assembly.GetManifestResourceStream(DbName);
            if (dbAssetStream != null) await dbAssetStream.CopyToAsync(dbFileStream);
            _preferenceStorage.Set(WordStorageConstant.VersionKey, WordStorageConstant.Version);
        }

        public async Task<ObjectWord> GetWordAsync(int id) =>
            await Connection.Table<ObjectWord>().FirstOrDefaultAsync(p => p.Id == id);

        public async Task<ObjectWord> GetRandomWordAsync()
        {
            var words = await GetWordsAsync(Expression.Lambda<Func<ObjectWord, bool>>(Expression.Constant(true), Expression.Parameter(typeof(ObjectWord), "p")), new Random().Next(5000), 1);
            return words.First();
        }

        public async Task<IList<ObjectWord>> GetWordsAsync(Expression<Func<ObjectWord, bool>> where, int skip, int take) =>
            await Connection.Table<ObjectWord>().Where(where).Skip(skip).Take(take).ToListAsync();

        public async Task SaveWordAsync(ObjectWord objectWord) =>
            await Connection.InsertOrReplaceAsync(objectWord);

        public async Task<IList<ObjectWord>> GetWordQuizOptionsAsync(ObjectWord correctWord)
        {
            var random = new Random();
            var wordList = await Connection.Table<ObjectWord>().Where(p => p.Word != correctWord.Word).Skip(random.Next(5000)).Take(3).ToListAsync();
            wordList.Insert(random.Next(0, 3), correctWord);
            return wordList;
        }

        public async Task CloseAsync() => await Connection.CloseAsync();
    }

    public static class WordStorageConstant
    {
        public const string VersionKey = nameof(WordStorageConstant) + "." + nameof(Version);
        public const int Version = 1;
    }

    public class BaicizhanListResult
    {
        public int Total { get; set; }
        public string[] List { get; set; }
    }

    public class BaicizhanWordResult
    {
        public string word { get; set; }
        public string accent { get; set; }
        public string mean_cn { get; set; }
        public string mean_en { get; set; }
        public string sentence { get; set; }
        public string sentence_trans { get; set; }
        public string sentence_phrase { get; set; }
        public string word_etyma { get; set; }
        public ClozeData cloze_data { get; set; }
    }

    public class ClozeData
    {
        public string syllable { get; set; }
        public string cloze { get; set; }
        public string[] options { get; set; }
        public string[][] tips { get; set; }
    }
}
