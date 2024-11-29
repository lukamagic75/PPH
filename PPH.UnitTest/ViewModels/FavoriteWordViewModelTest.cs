using Moq;
using PPH.Library.Models;
using PPH.Library.Services;
using PPH.Library.ViewModels;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPH.UnitTest.ViewModels
{
    public class FavoriteWordViewModelTest
    {
        // 测试 OnInitializedAsync 方法是否能正确加载 FavoriteWords
        [Fact]
        public async Task OnInitializedAsync_LoadsFavoriteWords()
        {
            // Arrange - 创建假数据
            var wordListToReturn = new List<ObjectWord>();
            for (var i = 1; i <= 5; i++)
            {
                wordListToReturn.Add(new ObjectWord { Id = i, Word = "TestWord" + i });
            }

            var favoriteListToReturn = wordListToReturn.Select(p => new FavoriteWord
            {
                WordId = p.Id,
                IsFavorite = true,
                Timestamp = DateTime.Now
            }).ToList();

            var favoriteStorageMock = new Mock<IFavoriteWordStorage>();
            favoriteStorageMock.Setup(p => p.GetFavoriteListAsync())
                .ReturnsAsync(favoriteListToReturn);
            var mockFavoriteStorage = favoriteStorageMock.Object;

            var wordStorageMock = new Mock<IWordStorage>();
            foreach (var word in wordListToReturn)
            {
                wordStorageMock.Setup(m => m.GetWordAsync(word.Id))
                    .ReturnsAsync(word);
            }
            var mockWordStorage = wordStorageMock.Object;

            var favoritePageViewModel = new FavoriteWordViewModel(mockFavoriteStorage, mockWordStorage, null);

            // Act - 调用 OnInitializedAsync 加载数据
            await favoritePageViewModel.OnInitializedAsync();

            // Assert - 确保 FavoriteWords 已经加载到集合中
            Assert.Equal(favoriteListToReturn.Count, favoritePageViewModel.FavouriteWordCollection.Count);
            foreach (var favorite in favoriteListToReturn)
            {
                var item = favoritePageViewModel.FavouriteWordCollection.FirstOrDefault(p => p.Favorite.WordId == favorite.WordId);
                Assert.NotNull(item);  // 确保集合中存在对应的项
            }
        }

        // 测试点击单词时是否正确跳转到详情页
        [Fact]
        public async Task WordTappedCommandFunction_Default()
        {
            // Arrange - 创建导航服务模拟
            var contentNavigationServiceMock = new Mock<IContentNavigationService>();
            var mockContentNavigationService = contentNavigationServiceMock.Object;

            var favoriteStorageMock = new Mock<IFavoriteWordStorage>();
            var mockFavoriteStorage = favoriteStorageMock.Object;

            var favoritePageViewModel = new FavoriteWordViewModel(mockFavoriteStorage, null, mockContentNavigationService);
            
            var wordFavoriteToNavigate = new FavoriteWordCombination
            {
                ObjectWord = new ObjectWord { Id = 1, Word = "TestWord" }
            };

            // Act - 点击单词，触发跳转
            favoritePageViewModel.ShowWordDetail(wordFavoriteToNavigate.ObjectWord);

            // Assert - 确保调用了导航方法
            contentNavigationServiceMock.Verify(
                p => p.NavigateTo(ContentNavigationConstant.DetailView, wordFavoriteToNavigate.ObjectWord), Times.Once);
        }

        // 测试 FavoriteStorage 的更新事件是否正确处理
        [Fact]
        public void FavoriteStorageOnUpdated_Default()
        {
            // Arrange - 创建假数据
            var wordFavoriteList = new List<FavoriteWordCombination>();
            for (int i = 1; i <= 5; i++)
            {
                wordFavoriteList.Add(new FavoriteWordCombination
                {
                    Favorite = new FavoriteWord
                    {
                        WordId = i,
                        IsFavorite = true,
                        Timestamp = DateTime.Now.Subtract(TimeSpan.FromMinutes(i))
                    }
                });
            }

            var favoriteUpdated = new FavoriteWord
            {
                WordId = wordFavoriteList[2].Favorite.WordId,
                IsFavorite = false,
                Timestamp = wordFavoriteList[2].Favorite.Timestamp
            };

            var poetryToReturn = new ObjectWord { Id = favoriteUpdated.WordId };

            var wordStorageMock = new Mock<IWordStorage>();
            wordStorageMock.Setup(p => p.GetWordAsync(poetryToReturn.Id))
                .ReturnsAsync(poetryToReturn);

            var favoriteStorageMock = new Mock<IFavoriteWordStorage>();
            favoriteStorageMock.Setup(p => p.GetFavoriteListAsync())
                .ReturnsAsync(wordFavoriteList.Select(p => p.Favorite).ToList());
            var mockFavoriteStorage = favoriteStorageMock.Object;

            var favoritePageViewModel = new FavoriteWordViewModel(mockFavoriteStorage, wordStorageMock.Object, null);

            // Act - 加载初始数据
            favoritePageViewModel.FavouriteWordCollection.AddRange(wordFavoriteList);

            // Simulate FavoriteStorage updated event
            favoriteStorageMock.Raise(p => p.Updated += null, mockFavoriteStorage, new FavoriteStorageUpdatedEventArgs(favoriteUpdated));

            // Assert - 确保更新后的 favorite 被从集合中移除
            Assert.Equal(wordFavoriteList.Count - 1, favoritePageViewModel.FavouriteWordCollection.Count);
            Assert.DoesNotContain(favoritePageViewModel.FavouriteWordCollection, p => p.Favorite.WordId == favoriteUpdated.WordId);

            // Act - 更新 favorite，将其重新加入集合
            favoriteUpdated.IsFavorite = true;
            favoriteStorageMock.Raise(p => p.Updated += null, mockFavoriteStorage, new FavoriteStorageUpdatedEventArgs(favoriteUpdated));

            // Assert - 确保更新后的 favorite 被重新加入集合
            Assert.Equal(wordFavoriteList.Count, favoritePageViewModel.FavouriteWordCollection.Count);
            var updatedFavorite = favoritePageViewModel.FavouriteWordCollection.FirstOrDefault(p => p.Favorite.WordId == favoriteUpdated.WordId);
            Assert.NotNull(updatedFavorite);
        }
    }
}
