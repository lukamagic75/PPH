using System.Linq.Expressions;
using PPH.Library.Models;

namespace PPH.Library.Services;

public interface IWordStorage {
    bool IsInitialized { get; }

    Task InitializeAsync();

    Task InitializeAsyncForFirstTime(); //只在第一次获取单词保存为数据库表时用到
    
    Task<ObjectWord> GetWordAsync(int id);
    
    Task<ObjectWord> GetRandomWordAsync();
    
    Task<IList<ObjectWord>> GetWordsAsync(
        Expression<Func<ObjectWord, bool>> where, int skip, int take);
    
    Task SaveWordAsync(ObjectWord wordObject);
    
    // 获取与correctWord不相同的三个单词，四个单词一起作为List返回，作为单词测验的选项
    Task<IList<ObjectWord>> GetWordQuizOptionsAsync(ObjectWord correctWord);
}