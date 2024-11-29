using PPH.Library.Helpers;
using PPH.Library.Models;
using PPH.Library.Services;
using SQLite;

namespace PPH.Library.Services;

public class FavoriteWordStorage : IFavoriteWordStorage {
    public const string DbName = "favoriteworddb.sqlite3";
    public static readonly string FavoriteWordDbPath =
        PathHelper.GetLocalFilePath(DbName);
    
    public event EventHandler<FavoriteStorageUpdatedEventArgs>? Updated;
    
    private SQLiteAsyncConnection _connection;
    private SQLiteAsyncConnection Connection =>
        _connection ??= new SQLiteAsyncConnection(FavoriteWordDbPath);
    
    private readonly IPreferenceStorage _preferenceStorage;

    public FavoriteWordStorage(IPreferenceStorage preferenceStorage) {
        _preferenceStorage = preferenceStorage;
    }
    
    public bool IsInitialized =>
        _preferenceStorage.Get(FavoriteWordStorageConstant.VersionKey,
            default(int)) == FavoriteWordStorageConstant.Version &&
        File.Exists(FavoriteWordDbPath);
    
    public async Task InitializeAsync() {
        Console.WriteLine("Initializing FavoriteWordStorage...");
        await Connection.CreateTableAsync<FavoriteWord>();
        _preferenceStorage.Set(FavoriteWordStorageConstant.VersionKey,
            FavoriteWordStorageConstant.Version);
    }

    public async Task<FavoriteWord> GetFavoriteAsync(int wordId) {
        return await Connection.Table<FavoriteWord>()
            .FirstOrDefaultAsync(p => p.WordId == wordId);
    }

    public async Task<IEnumerable<FavoriteWord>> GetFavoriteListAsync() {
        return await Connection.Table<FavoriteWord>().Where(p => p.IsFavorite)
            .OrderByDescending(p => p.Timestamp).ToListAsync();
    }

    public async Task SaveFavoriteAsync(FavoriteWord favorite) {
        favorite.Timestamp = DateTime.Now;
        await Connection.InsertOrReplaceAsync(favorite);
        // 触发更新事件
        Updated?.Invoke(this, new FavoriteStorageUpdatedEventArgs(favorite));
    }

    public async Task CloseAsync() {
        await Connection.CloseAsync();
    }
}

public static class FavoriteWordStorageConstant {
    public const string VersionKey =
        nameof(FavoriteWordStorageConstant) + "." + nameof(Version);

    public const int Version = 1;
}
