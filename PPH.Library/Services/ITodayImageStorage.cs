using PPH.Library.Models;

namespace PPH.Library.Services;

public interface ITodayImageStorage {
    Task<TodayImage> GetTodayImageAsync(bool isIncludingImageStream);

    Task SaveTodayImageAsync(TodayImage todayImage, bool isSavingExpiresAtOnly);
}