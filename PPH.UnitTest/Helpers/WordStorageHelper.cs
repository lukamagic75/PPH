using PPH.Library.Services;
using Moq;

namespace PPH.UnitTest.Helpers;

public class WordStorageHelper {
    public static void RemoveDatabaseFile() =>
        File.Delete(WordStorage.WordDbPath);

    public static async Task<WordStorage> GetInitializedWordStorage() {
        var preferenceStorageMock = new Mock<IPreferenceStorage>();
        var alertStorageMock = new Mock<IAlertService>();
        preferenceStorageMock.Setup(p =>
            p.Get(WordStorageConstant.VersionKey, -1)).Returns(-1);
        var mockPreferenceStorage = preferenceStorageMock.Object;
        var mockAlertService = alertStorageMock.Object;
        var wordStorage = new WordStorage(mockPreferenceStorage, mockAlertService);
        await wordStorage.InitializeAsync();
        return wordStorage;
    }
}