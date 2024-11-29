using Moq;
using PPH.Library.Models;
using PPH.Library.Services;
using PPH.Library.ViewModels;
using Xunit;

namespace PPH.UnitTest.ViewModels
{
    public class DetailViewModelTest
    {
        private readonly Mock<IMenuNavigationService> _menuNavigationServiceMock;
        private readonly Mock<IFavoriteWordStorage> _wordFavoriteStorageMock;
        private readonly DetailViewModel _viewModel;

        public DetailViewModelTest()
        {
            // 使用 Moq 模拟依赖项
            _menuNavigationServiceMock = new Mock<IMenuNavigationService>();
            _wordFavoriteStorageMock = new Mock<IFavoriteWordStorage>();

            // 创建 ViewModel 实例，并传入模拟的依赖项
            _viewModel = new DetailViewModel(_menuNavigationServiceMock.Object, _wordFavoriteStorageMock.Object);
        }

        [Fact]
        public async Task OnLoadedAsync_InitializesFavoriteAndSetsLoadingState()
        {
            // Arrange
            var word = new ObjectWord { Id = 1, Word = "TestWord", CnMeaning = "测试词" };
            _viewModel.SetParameter(word);  // 设置当前单词

            // 修改：使用 WordId 来匹配 ObjectWord 的 Id
            var mockFavorite = new FavoriteWord { WordId = word.Id };

            // 模拟 _wordFavoriteStorage.GetFavoriteAsync 方法的返回值
            _wordFavoriteStorageMock
                .Setup(s => s.GetFavoriteAsync(word.Id))
                .ReturnsAsync(mockFavorite);

            // Act
            await _viewModel.OnLoadedAsync();  // 调用 ViewModel 的 OnLoadedAsync 方法

            // Assert
            Assert.False(_viewModel.IsLoading);  // 检查 loading 状态是否被正确设置
            Assert.Equal(mockFavorite, _viewModel.Favorite);  // 检查 Favorite 是否被正确赋值
            _wordFavoriteStorageMock.Verify(s => s.GetFavoriteAsync(word.Id), Times.Once);  // 确保方法被调用一次
        }

        [Fact]
        public async Task FavoriteSwitchClickedAsync_SavesFavoriteAndSetsLoadingState()
        {
            // Arrange
            var word = new ObjectWord { Id = 1, Word = "TestWord", CnMeaning = "测试词" };
            _viewModel.SetParameter(word);  // 设置当前单词

            var favorite = new FavoriteWord { WordId = word.Id };
            _viewModel.Favorite = favorite;

            // 模拟 _wordFavoriteStorage.SaveFavoriteAsync 方法
            _wordFavoriteStorageMock
                .Setup(s => s.SaveFavoriteAsync(favorite))
                .Returns(Task.CompletedTask);

            // Act
            await _viewModel.FavoriteSwitchClickedAsync();  // 调用切换收藏状态的方法

            // Assert
            Assert.False(_viewModel.IsLoading);  // 检查 loading 状态是否被正确更新
            _wordFavoriteStorageMock.Verify(s => s.SaveFavoriteAsync(favorite), Times.Once);  // 确保保存方法被调用一次
        }

        [Fact]
        public void Query_NavigatesToQueryWordView()
        {
            // Arrange
            var word = new ObjectWord { Id = 1, Word = "TestWord", CnMeaning = "测试词" };
            _viewModel.SetParameter(word);  // 设置当前单词

            // Act
            _viewModel.Query();  // 调用查询方法

            // Assert
            _menuNavigationServiceMock.Verify(m => m.NavigateTo(MenuNavigationConstant.QueryWordView, It.IsAny<QueryWord>()), Times.Once);  // 确保导航方法被调用一次
        }
    }
}
