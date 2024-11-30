using PPH.Library.Services;
using PPH.Library.ViewModels;
using Moq;
using Xunit;

namespace PPH.UnitTest.ViewModels
{
    public class MainWindowViewModelTest
    {
        [Fact]
        public void OnInitialized_NotInitialized()
        {
            // Arrange
            var wordStorageMock = new Mock<IWordStorage>();
            wordStorageMock.Setup(p => p.IsInitialized).Returns(false);  // Mock IsInitialized 为 false
            var mockWordStorage = wordStorageMock.Object;

            var favoriteStorageMock = new Mock<IFavoriteWordStorage>();
            favoriteStorageMock.Setup(p => p.IsInitialized).Returns(false);  // Mock FavoriteWordStorage 为 false
            var mockFavoriteStorage = favoriteStorageMock.Object;
            
            var memoStorageMock = new Mock<IMemoStorage>();
            memoStorageMock.Setup(p => p.IsInitialized).Returns(false);
            var mockMemoStorage = memoStorageMock.Object;


            var rootNavigationServiceMock = new Mock<IRootNavigationService>();
            var mockRootNavigationService = rootNavigationServiceMock.Object;
            
            
            var mainWindowViewModel = new MainWindowViewModel(
                mockWordStorage, mockMemoStorage, mockRootNavigationService, mockFavoriteStorage);

            // Act
            mainWindowViewModel.OnInitialized();  // 调用同步方法 OnInitialized

            // Assert
            wordStorageMock.Verify(p => p.IsInitialized, Times.Once);  // 验证 IsInitialized 是否被访问
            favoriteStorageMock.Verify(p => p.IsInitialized, Times.Never);  // 确保没有调用 FavoriteStorage 的 IsInitialized
            rootNavigationServiceMock.Verify(
                p => p.NavigateTo(RootNavigationConstant.InitializationView), Times.Once);  // 验证导航到 InitializationView
        }

        [Fact]
        public void OnInitialized_Initialized()
        {
            // Arrange
            var wordStorageMock = new Mock<IWordStorage>();
            wordStorageMock.Setup(p => p.IsInitialized).Returns(true);  // Mock IsInitialized 为 true
            var mockWordStorage = wordStorageMock.Object;

            var favoriteStorageMock = new Mock<IFavoriteWordStorage>();
            favoriteStorageMock.Setup(p => p.IsInitialized).Returns(true);  // Mock FavoriteWordStorage 为 true
            var mockFavoriteStorage = favoriteStorageMock.Object;

            var rootNavigationServiceMock = new Mock<IRootNavigationService>();
            var mockRootNavigationService = rootNavigationServiceMock.Object;
            
            var memoStorageMock = new Mock<IMemoStorage>();
            memoStorageMock.Setup(p => p.IsInitialized).Returns(false);
            var mockMemoStorage = memoStorageMock.Object;


            var mainWindowViewModel = new MainWindowViewModel(
                mockWordStorage, mockMemoStorage, mockRootNavigationService, mockFavoriteStorage);

            // Act
            mainWindowViewModel.OnInitialized();  // 调用同步方法 OnInitialized

            // Assert
            wordStorageMock.Verify(p => p.IsInitialized, Times.Once);  // 验证 IsInitialized 是否被访问
            favoriteStorageMock.Verify(p => p.IsInitialized, Times.Once);  // 验证 FavoriteStorage 的 IsInitialized 是否被访问
            rootNavigationServiceMock.Verify(
                p => p.NavigateTo(RootNavigationConstant.MainView), Times.Once);  // 验证导航到 MainView
        }
    }
}
