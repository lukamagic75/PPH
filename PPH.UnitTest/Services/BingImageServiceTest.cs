using PPH.Library.Models;
using PPH.Library.Services;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PPH.UnitTest.Services
{
    public class BingImageServiceTest
    {
        [Fact(Skip = "依赖远程服务的测试")]
        public async Task CheckUpdateAsync_TodayImageNotExpired()
        {
            var todayImageToReturn = new TodayImage
            {
                FullStartDate = "19700101",
                ExpiresAt = DateTime.Now + TimeSpan.FromHours(1), // 设置当前未过期
                Copyright = "Copyright",
                CopyrightLink = "Today's image"
            };

            var todayImageStorageMock = new Mock<ITodayImageStorage>();
            todayImageStorageMock.Setup(p => p.GetTodayImageAsync(false))
                .ReturnsAsync(todayImageToReturn);
            var mockTodayImageStorage = todayImageStorageMock.Object;

            var alertServiceMock = new Mock<IAlertService>();
            var mockAlertService = alertServiceMock.Object;

            var todayImageService = new BingImageService(mockAlertService, mockTodayImageStorage);
            var checkUpdateResult = await todayImageService.CheckUpdateAsync();

            // 验证结果：应该没有更新
            Assert.False(checkUpdateResult.HasUpdate);

            // 验证 GetTodayImageAsync 被调用了一次
            todayImageStorageMock.Verify(p => p.GetTodayImageAsync(false), Times.Once);

            // 验证没有调用 SaveTodayImageAsync
            todayImageStorageMock.Verify(
                p => p.SaveTodayImageAsync(It.IsAny<TodayImage>(), It.IsAny<bool>()), Times.Never);

            // 验证没有弹出警告
            alertServiceMock.Verify(
                p => p.AlertAsync(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact(Skip = "依赖远程服务的测试")]
        public async Task CheckUpdateAsync_TodayImageExpired()
        {
            var todayImageToReturn = new TodayImage
            {
                FullStartDate = "19700101",
                ExpiresAt = DateTime.Now + TimeSpan.FromHours(1), // 设置当前未过期
                Copyright = "Copyright",
                CopyrightLink = "Today's image"
            };

            var todayImageStorageMock = new Mock<ITodayImageStorage>();
            todayImageStorageMock.Setup(p => p.GetTodayImageAsync(false))
                .ReturnsAsync(todayImageToReturn);
            var mockTodayImageStorage = todayImageStorageMock.Object;

            var alertServiceMock = new Mock<IAlertService>();
            var mockAlertService = alertServiceMock.Object;

            var todayImageService = new BingImageService(mockAlertService, mockTodayImageStorage);
            var checkUpdateResult = await todayImageService.CheckUpdateAsync();

            // 验证结果：应该有更新
            Assert.True(checkUpdateResult.HasUpdate);

            // 验证 GetTodayImageAsync 被调用了一次
            todayImageStorageMock.Verify(p => p.GetTodayImageAsync(false), Times.Once);

            // 验证 SaveTodayImageAsync 被调用一次，保存新的 TodayImage
            todayImageStorageMock.Verify(
                p => p.SaveTodayImageAsync(checkUpdateResult.TodayImage, false),
                Times.Once);

            // 验证没有弹出警告
            alertServiceMock.Verify(
                p => p.AlertAsync(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }
    }
}
