using PPH.Library.Models;

namespace PPH.Library.Services;

public interface IFavoriteWordStorage {
    bool IsInitialized { get; }

    Task InitializeAsync();

    Task<FavoriteWord> GetFavoriteAsync(int wordId);

    Task<IEnumerable<FavoriteWord>> GetFavoriteListAsync();

    Task SaveFavoriteAsync(FavoriteWord favorite);

    event EventHandler<FavoriteStorageUpdatedEventArgs> Updated;
}

public class FavoriteStorageUpdatedEventArgs : EventArgs {
    public FavoriteWord UpdatedFavorite { get; }

    public FavoriteStorageUpdatedEventArgs(FavoriteWord favorite) {
        UpdatedFavorite = favorite;
    }
}