using Moq;
using System.Threading.Tasks;
using Xunit;
using PPH.Library.Services;

namespace PPH.UnitTest.Services
{
    public class TranslateServiceTest
    {
        [Fact]
        public async Task Translate_ReturnIsNotNullOrWhiteSpace()
        {
            // 创建一个模拟的 IAlertService
            var alertServiceMock = new Mock<IAlertService>();
            var mockAlertService = alertServiceMock.Object;

            // 创建 TranslateService 实例
            var translateService = new TranslateService(mockAlertService);

            // 定义待翻译的源文本
            var sourceText = "Good morning";

            // 调用 Translate 方法进行翻译
            var result = await translateService.Translate(sourceText, "auto", "zh");

            // 验证翻译结果是否非空
            Assert.False(string.IsNullOrWhiteSpace(result));

            // 验证翻译是否正确
            Assert.Equal("早上好", result);

            // 验证 AlertService 中的 AlertAsync 方法没有被调用
            alertServiceMock.Verify(
                p => p.AlertAsync(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }
    }
}