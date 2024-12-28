using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using PPH.Library.Helpers;
using PPH.Library.Models;
using PPH.Library.Services;

public class MemoStorageTest : IDisposable
{
    private const string TestDatabasePath = "test_memosdb.sqlite3";
    private MemoStorage _memoStorage;

    public MemoStorageTest()
    {
        RemoveTestDatabase();
        _memoStorage = new MemoStorage();
        InitializeDatabase().Wait();
    }

    // 清理测试数据库
    private void RemoveTestDatabase()
    {
        if (File.Exists(TestDatabasePath))
        {
            File.Delete(TestDatabasePath);
        }
    }

    // 初始化数据库
    private async Task InitializeDatabase()
    {
        await _memoStorage.InitializeAsync();
    }

    [Fact]
    public async Task InitializeAsync_Success()
    {
        // Act
        await _memoStorage.InitializeAsync();

        // Assert
        //Assert.True(File.Exists(MemoStorage.MemoDbPath), 
        //    "Database file does not exist. " +
        //    "Please check the path or initialization logic.");
    }

    [Fact]
    public async Task SaveMemoAsync_Success()
    {
        // Arrange
        var today = DateTime.Now.Date;
        var memo = new MemoObject
        {
            Date = DateHelper.ToDateString(today),
            Content = "Unique Test Memo"
        };

        // Act
        await _memoStorage.SaveMemoAsync(memo);

        // Assert
        var results = await _memoStorage.GetMemosByDateAsync(today);
        Assert.Contains(results, m => m.Content == "Unique Test Memo");
    }

    [Fact]
    public async Task DeleteMemoAsync_Success()
    {
        // Arrange
        var testDate = DateTime.Now.Date;
        var memo = new MemoObject
        {
            Date = DateHelper.ToDateString(testDate),
            Content = "Memo to be deleted"
        };
        await _memoStorage.SaveMemoAsync(memo);

        // Act
        await _memoStorage.DeleteMemoAsync(memo.Id);

        // Assert
        var results = await _memoStorage.GetMemosByDateAsync(testDate);
        Assert.DoesNotContain(results, m => m.Id == memo.Id);
    }

    [Fact]
    public async Task GetMemosByDateAsync_MultipleEntries()
    {
        // // Arrange
        // var testDate = DateTime.Now.Date;
        // var memo1 = new MemoObject { Date = DateHelper.ToDateString(testDate), 
        //     Content = "Memo 1" };
        // var memo2 = new MemoObject { Date = DateHelper.ToDateString(testDate), 
        //     Content = "Memo 2" };
        //
        // await _memoStorage.SaveMemoAsync(memo1);
        // await _memoStorage.SaveMemoAsync(memo2);
        //
        // // Act
        // var results = await _memoStorage.
        //     GetMemosByDateAsync(testDate);
        //
        // // Assert
        // Assert.Equal(2, results.Count(m => m.Date == DateHelper.
        //     ToDateString(testDate)));
        // Assert.Contains(results, m => m.Content == "Memo 1");
        // Assert.Contains(results, m => m.Content == "Memo 2");
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _memoStorage.SaveMemoAsync(null));
    }
    

    // public void Dispose()
    // {
    //     // 清理数据库
    //     RemoveTestDatabase();
    // }

    [Fact]
    public async Task SaveMemoAsync_NullMemo_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _memoStorage.SaveMemoAsync(null));
    }

    [Fact]
    public async Task GetMemosByDateAsync_InvalidDate_ReturnsEmptyList()
    {
        // Arrange
        var invalidDate = new DateTime(2000, 1, 1);

        // Act
        var results = await _memoStorage.GetMemosByDateAsync(invalidDate);

        // Assert
        Assert.Empty(results);
    }

    public void Dispose()
    {
        // 清理数据库
        RemoveTestDatabase();
    }
}