using PPH.Library.Models;

namespace PPH.Library.Services;

public interface ITodayImageService {
    Task<TodayImage> GetTodayImageAsync(); 
    
    Task<TodayImageServiceCheckUpdateResult> CheckUpdateAsync();
    
    Task<TodayImage> GetRandomImageAsync(); 
}

public class TodayImageServiceCheckUpdateResult {
    public bool HasUpdate { get; set; }

    public TodayImage TodayImage { get; set; } = new();
}