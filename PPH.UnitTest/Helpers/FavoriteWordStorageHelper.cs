using PPH.Library.Helpers;
using PPH.Library.Services;

namespace PPH.UnitTest.Helpers;

public class FavoriteWordStorageHelper {
    public static void RemoveDatabaseFile() {
        var keyPath = PathHelper.GetLocalFilePath(FavoriteWordStorageConstant.VersionKey);
        File.Delete(keyPath);
        File.Delete(FavoriteWordStorage.FavoriteWordDbPath);
    }
}