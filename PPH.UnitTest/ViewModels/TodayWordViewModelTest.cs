using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using PPH.Library.Models;
using PPH.Library.Services;
using PPH.Library.ViewModels;
using Xunit;

namespace PPH.UnitTest.ViewModels
{
    public class TodayWordViewModelTest
    {
        [Fact]
        public async Task OnInitialized_ImageUpdated()
        {
            var oldTodayImageToReturn = new TodayImage();
            var newTodayImageToReturn = new TodayImage();
            var updateResultToReturn = new TodayImageServiceCheckUpdateResult
            {
                HasUpdate = true, TodayImage = newTodayImageToReturn
            };

            var todayImageServiceMock = new Mock<ITodayImageService>();
            todayImageServiceMock.Setup(p => p.GetTodayImageAsync())
                .ReturnsAsync(oldTodayImageToReturn);
            todayImageServiceMock.Setup(p => p.CheckUpdateAsync())
                .ReturnsAsync(updateResultToReturn);
            var mockImageService = todayImageServiceMock.Object;

            var todayWordToReturn = new ObjectWord();
            var wordStorageMock = new Mock<IWordStorage>();
            wordStorageMock.Setup(p => p.GetRandomWordAsync())
                .ReturnsAsync(todayWordToReturn);
            var mockWordStorage = wordStorageMock.Object;

            // 构造TodayWordViewModel并自动调用OnInitialized
            var todayWordViewModel = new TodayWordViewModel(
                mockWordStorage, mockImageService, null, null);

            var todayImageList = new List<TodayImage>();
            var isLoadingList = new List<bool>();
            todayWordViewModel.PropertyChanged += (sender, args) =>
            {
                switch (args.PropertyName)
                {
                    case nameof(TodayWordViewModel.TodayImage):
                        todayImageList.Add(todayWordViewModel.TodayImage);
                        break;
                    case nameof(TodayWordViewModel.IsLoading):
                        isLoadingList.Add(todayWordViewModel.IsLoading);
                        break;
                }
            };

            // 等待异步操作完成
            while (todayImageList.Count != 2 || isLoadingList.Count != 2)
            {
                await Task.Delay(100);
            }

            // 断言
            Assert.Same(oldTodayImageToReturn, todayImageList[0]);
            Assert.Same(newTodayImageToReturn, todayImageList[1]);
            Assert.True(isLoadingList[0]);
            Assert.False(isLoadingList[1]);
            Assert.Same(todayWordToReturn, todayWordViewModel.TodayWord);

            todayImageServiceMock.Verify(p => p.GetTodayImageAsync(), Times.Once);
            todayImageServiceMock.Verify(p => p.CheckUpdateAsync(), Times.Once);
            wordStorageMock.Verify(p => p.GetRandomWordAsync(), Times.Once);
        }

        [Fact]
        public async Task OnInitialized_ImageNotUpdated()
        {
            var oldTodayImageToReturn = new TodayImage();
            var updateResultToReturn = new TodayImageServiceCheckUpdateResult
            {
                HasUpdate = false
            };

            var todayImageServiceMock = new Mock<ITodayImageService>();
            todayImageServiceMock.Setup(p => p.GetTodayImageAsync())
                .ReturnsAsync(oldTodayImageToReturn);
            todayImageServiceMock.Setup(p => p.CheckUpdateAsync())
                .ReturnsAsync(updateResultToReturn);
            var mockImageService = todayImageServiceMock.Object;

            var todayWordToReturn = new ObjectWord();
            var wordStorageMock = new Mock<IWordStorage>();
            wordStorageMock.Setup(p => p.GetRandomWordAsync())
                .ReturnsAsync(todayWordToReturn);
            var mockWordStorage = wordStorageMock.Object;

            // 构造TodayWordViewModel并自动调用OnInitialized
            var todayWordViewModel = new TodayWordViewModel(
                mockWordStorage, mockImageService, null, null);

            var todayImageList = new List<TodayImage>();
            var todayWordList = new List<ObjectWord>();
            todayWordViewModel.PropertyChanged += (sender, args) =>
            {
                switch (args.PropertyName)
                {
                    case nameof(TodayWordViewModel.TodayImage):
                        todayImageList.Add(todayWordViewModel.TodayImage);
                        break;
                    case nameof(TodayWordViewModel.TodayWord):
                        todayWordList.Add(todayWordViewModel.TodayWord);
                        break;
                }
            };

            // 等待异步操作完成
            while (todayImageList.Count != 1 || todayWordList.Count != 1)
            {
                await Task.Delay(100);
            }

            // 断言
            Assert.Same(oldTodayImageToReturn, todayImageList[0]);

            todayImageServiceMock.Verify(p => p.GetTodayImageAsync(), Times.Once);
            todayImageServiceMock.Verify(p => p.CheckUpdateAsync(), Times.Once);
            wordStorageMock.Verify(p => p.GetRandomWordAsync(), Times.Once);
        }

        [Fact]
        public void ShowDetailCommandFunction_Default()
        {
            var contentNavigationServiceMock = new Mock<IContentNavigationService>();
            var mockContentNavigationService = contentNavigationServiceMock.Object;

            // 创建TodayWordViewModel
            var todayWordViewModel = new TodayWordViewModel(null, null,
                mockContentNavigationService, null);
            
            // 调用ShowDetail()方法
            todayWordViewModel.ShowDetail();
            
            // 验证是否正确调用了NavigateTo方法
            contentNavigationServiceMock.Verify(
                p => p.NavigateTo(ContentNavigationConstant.DetailView, null),
                Times.Once);
        }
    }
}
