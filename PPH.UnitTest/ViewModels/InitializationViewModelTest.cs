using Moq;
using PPH.Library.Services;
using PPH.Library.ViewModels;
using System.Threading.Tasks;
using Xunit;

namespace PPH.UnitTest.ViewModels
{
    public class InitializationViewModelTest
    {
        [Fact]
        public async Task OnInitializedAsync_NotInitialized()
        {
            // Arrange
            var wordStorageMock = new Mock<IWordStorage>();
            wordStorageMock.Setup(p => p.IsInitialized).Returns(false);
            var mockWordStorage = wordStorageMock.Object;
            
            var memoStorageMock = new Mock<IMemoStorage>();
            memoStorageMock.Setup(p => p.IsInitialized).Returns(false);
            var mockMemoStorage = memoStorageMock.Object;
            
            var musicStorageMock = new Mock<IMusicStorage>();
            musicStorageMock.Setup(p => p.IsInitialized).Returns(false);
            var mockMusicStorage = musicStorageMock.Object;

            var favoriteStorageMock = new Mock<IFavoriteWordStorage>();
            favoriteStorageMock.Setup(p => p.IsInitialized).Returns(false);
            var mockFavoriteStorage = favoriteStorageMock.Object;

            var rootNavigationServiceMock = new Mock<IRootNavigationService>();
            var mockRootNavigationService = rootNavigationServiceMock.Object;

            
            var initializationViewModel = new InitializationViewModel(
                mockWordStorage, mockMemoStorage, mockMusicStorage, mockRootNavigationService, mockFavoriteStorage);

            // Act
            await initializationViewModel.OnInitializedAsync();

            // Assert
            // Verify that IsInitialized was checked and InitializeAsync was called for both services
            wordStorageMock.Verify(p => p.IsInitialized, Times.Once);
            wordStorageMock.Verify(p => p.InitializeAsync(), Times.Once);

            favoriteStorageMock.Verify(p => p.IsInitialized, Times.Once);
            favoriteStorageMock.Verify(p => p.InitializeAsync(), Times.Once);

            // Verify that after initialization, the navigation to MainView happens
            rootNavigationServiceMock.Verify(
                p => p.NavigateTo(RootNavigationConstant.MainView), Times.Once);
        }

        [Fact]
        public async Task OnInitializedAsync_Initialized()
        {
            // Arrange
            var wordStorageMock = new Mock<IWordStorage>();
            wordStorageMock.Setup(p => p.IsInitialized).Returns(true);
            var mockWordStorage = wordStorageMock.Object;
            
            var memoStorageMock = new Mock<IMemoStorage>();
            memoStorageMock.Setup(p => p.IsInitialized).Returns(false);
            var mockMemoStorage = memoStorageMock.Object;
            
            var musicStorageMock = new Mock<IMusicStorage>();
            musicStorageMock.Setup(p => p.IsInitialized).Returns(false);
            var mockMusicStorage = musicStorageMock.Object;

            var favoriteStorageMock = new Mock<IFavoriteWordStorage>();
            favoriteStorageMock.Setup(p => p.IsInitialized).Returns(true);
            var mockFavoriteStorage = favoriteStorageMock.Object;

            var rootNavigationServiceMock = new Mock<IRootNavigationService>();
            var mockRootNavigationService = rootNavigationServiceMock.Object;

            var initializationViewModel = new InitializationViewModel(
                mockWordStorage,mockMemoStorage,mockMusicStorage, mockRootNavigationService, mockFavoriteStorage);

            // Act
            await initializationViewModel.OnInitializedAsync();

            // Assert
            // Verify that IsInitialized was checked, but InitializeAsync was not called because already initialized
            wordStorageMock.Verify(p => p.IsInitialized, Times.Once);
            wordStorageMock.Verify(p => p.InitializeAsync(), Times.Never);

            favoriteStorageMock.Verify(p => p.IsInitialized, Times.Once);
            favoriteStorageMock.Verify(p => p.InitializeAsync(), Times.Never);

            // Verify that after checking initialization, the navigation to MainView happens
            rootNavigationServiceMock.Verify(
                p => p.NavigateTo(RootNavigationConstant.MainView), Times.Once);
        }
    }
}
