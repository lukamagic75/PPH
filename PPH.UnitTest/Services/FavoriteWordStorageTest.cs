using PPH.Library.Models;
using PPH.Library.Services;
using PPH.UnitTest.Helpers;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PPH.UnitTest.Services
{
    public class FavoriteWordStorageTest : IDisposable
    {
        public FavoriteWordStorageTest()
        {
            // 在每个测试前移除数据库文件
            FavoriteWordStorageHelper.RemoveDatabaseFile();
        }

        public void Dispose()
        {
            // 在测试完成后移除数据库文件
            FavoriteWordStorageHelper.RemoveDatabaseFile();
        }

        [Fact]
        public async Task IsInitialized_Default()
        {
            var preferenceStorageMock = new Mock<IPreferenceStorage>();
            preferenceStorageMock
                .Setup(p => p.Get(FavoriteWordStorageConstant.VersionKey, default(int)))
                .Returns(FavoriteWordStorageConstant.Version);
            var mockPreferenceStorage = preferenceStorageMock.Object;

            var favoriteStorage = new FavoriteWordStorage(mockPreferenceStorage);
            await favoriteStorage.InitializeAsync();

            // 验证 FavoriteWordStorage 是否已经初始化
            Assert.True(favoriteStorage.IsInitialized);

            // 验证调用 Get 方法一次
            preferenceStorageMock.Verify(p => p.Get(FavoriteWordStorageConstant.VersionKey, default(int)),
                Times.Once);

            // 关闭存储
            await favoriteStorage.CloseAsync();
        }

        [Fact]
        public async Task InitializeAsync_Default()
        {
            var favoriteStorage = new FavoriteWordStorage(GetEmptyPreferenceStorage());

            // 确保数据库文件未创建
            Assert.False(File.Exists(FavoriteWordStorage.FavoriteWordDbPath));

            await favoriteStorage.InitializeAsync();

            // 确保数据库文件被创建
            Assert.True(File.Exists(FavoriteWordStorage.FavoriteWordDbPath));

            await favoriteStorage.CloseAsync();
        }

        [Fact]
        public async Task SaveFavoriteAsync_GetFavoriteAsync_Default()
        {
            var updated = false;
            FavoriteWord updatedFavorite = null;

            var favoriteStorage = new FavoriteWordStorage(GetEmptyPreferenceStorage());
            favoriteStorage.Updated += (_, args) =>
            {
                updated = true;
                updatedFavorite = args.UpdatedFavorite;
            };
            await favoriteStorage.InitializeAsync();

            var favoriteToSave = new FavoriteWord
            {
                WordId = 1,
                IsFavorite = true
            };

            // 保存收藏
            await favoriteStorage.SaveFavoriteAsync(favoriteToSave);

            // 获取保存的收藏并验证
            var favorite = await favoriteStorage.GetFavoriteAsync(favoriteToSave.WordId);
            Assert.Equal(favoriteToSave.WordId, favorite.WordId);
            Assert.Equal(favoriteToSave.IsFavorite, favorite.IsFavorite);
            Assert.NotEqual(default, favorite.Timestamp);
            Assert.True(updated);
            Assert.Same(favoriteToSave, updatedFavorite);

            // 再次保存收藏，确保时间戳更新
            await favoriteStorage.SaveFavoriteAsync(favoriteToSave);
            favorite = await favoriteStorage.GetFavoriteAsync(favoriteToSave.WordId);
            Assert.True(DateTime.Today < favorite.Timestamp);

            await favoriteStorage.CloseAsync();
        }

        [Fact]
        public async Task GetFavoritesAsync_Default()
        {
            var favoriteStorage = new FavoriteWordStorage(GetEmptyPreferenceStorage());
            await favoriteStorage.InitializeAsync();

            // 创建一些随机的收藏项
            var favoriteListToSave = new List<FavoriteWord>();
            var random = new Random();
            for (var i = 1; i <= 5; i++)
            {
                favoriteListToSave.Add(new FavoriteWord
                {
                    WordId = i,
                    IsFavorite = random.NextDouble() > 0.5
                });
                await Task.Delay(10);
            }

            // 将所有收藏项保存到存储中
            var favoriteDictionary = favoriteListToSave.Where(p => p.IsFavorite)
                .ToDictionary(p => p.WordId, p => true);

            foreach (var favoriteToSave in favoriteListToSave)
            {
                await favoriteStorage.SaveFavoriteAsync(favoriteToSave);
            }

            // 获取收藏列表并验证
            var favoriteList = await favoriteStorage.GetFavoriteListAsync();
            Assert.Equal(favoriteDictionary.Count, favoriteList.Count());
            foreach (var favorite in favoriteList)
            {
                Assert.True(favoriteDictionary.ContainsKey(favorite.WordId));
            }

            await favoriteStorage.CloseAsync();
        }

        private static IPreferenceStorage GetEmptyPreferenceStorage() =>
            new Mock<IPreferenceStorage>().Object;
    }
}
