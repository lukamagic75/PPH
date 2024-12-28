using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PPH.Library.Models;

namespace PPH.Library.Services;

public interface IMemoStorage {
    // 数据库是否初始化
    bool IsInitialized { get; }

    // 初始化数据库（用于首次加载）
    Task InitializeAsync();

    
    // 获取指定日期的备忘录
    Task<IList<MemoObject>> GetMemosByDateAsync(DateTime date);

    // 保存或更新备忘录
    Task SaveMemoAsync(MemoObject memoObject);

    // 删除备忘录
    Task DeleteMemoAsync(int id);

    
}