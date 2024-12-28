using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PPH.Library.Helpers;
using PPH.Library.Models;
using SQLite;

namespace PPH.Library.Services;

public class MemoStorage : IMemoStorage
{
    private const string DbName = "memosdb.sqlite3";

    public static readonly string MemoDbPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        DbName);

    private SQLiteAsyncConnection _connection;

    private SQLiteAsyncConnection Connection =>
        _connection ??= new SQLiteAsyncConnection(MemoDbPath);

    public bool IsInitialized { get; }

    public async Task InitializeAsync() {
        Console.WriteLine("初始化数据库...");
        await Connection.CreateTableAsync<MemoObject>();
        Console.WriteLine("数据库表已创建或已存在。");
    }
    

    public async Task SaveMemoAsync(MemoObject memoObject) {
        if (memoObject == null) throw new ArgumentNullException(nameof(memoObject));

        Console.WriteLine($"保存备忘录: 日期={memoObject.Date}, " 
                          + $"内容={memoObject.Content}");
        await Connection.InsertAsync(memoObject); // 插入新记录，而不是替换
    }
    
    public async Task DeleteMemoAsync(int id) {
        var memo = await Connection.FindAsync<MemoObject>(id);
        if (memo != null) {
            await Connection.DeleteAsync(memo);
        }
    }
    

    public async Task<IList<MemoObject>> GetMemosByDateAsync(DateTime date) {
        var dateString = DateHelper.ToDateString(date); // 转换查询日期为字符串
        Console.WriteLine($"查询日期字符串: {dateString}");

        try {
            var results = await Connection.Table<MemoObject>()
                .Where(m => m.Date == dateString)
                .ToListAsync();

            Console.WriteLine($"查询结果数量: {results.Count}");
            return results;
        }
        catch (Exception ex) {
            Console.WriteLine($"查询备忘事项时出错: {ex.Message}");
            throw;
        }
    }
}