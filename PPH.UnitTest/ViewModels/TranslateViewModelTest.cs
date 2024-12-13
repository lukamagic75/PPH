using System.Threading.Tasks;
using Moq;
using Xunit;
using PPH.Library.Services; // 适当替换命名空间
using PPH.Library.ViewModels; // 适当替换命名空间

namespace PPH.UnitTest.ViewModels
{
    public class TranslateViewModelTest
    {
        [Fact]
        public async Task TranslateAsync_Default()
        {
            // 创建 AlertService 的 Mock 对象
            var alertServiceMock = new Mock<IAlertService>();
            var mockAlertService = alertServiceMock.Object;

            // 创建 TranslateService，并将 Mock 的 AlertService 传入
            var translateService = new TranslateService(mockAlertService);

            // 创建 TranslateViewModel 实例
            var translateViewModel = new TranslateViewModel(translateService);

            // 测试翻译功能 - 英语翻译为中文
            translateViewModel.SourceText = "Good morning!";
            translateViewModel.LanguageType = TargetLanguageType.ToChineseType;
            await translateViewModel.TranslateAsync();
            Assert.Equal("早上好！", translateViewModel.TargetText); // 断言翻译结果是否正确

            // 测试翻译功能 - 中文翻译为英语
            translateViewModel.SourceText = "苹果";
            translateViewModel.LanguageType = TargetLanguageType.ToEnglishType;
            await translateViewModel.TranslateAsync();
            Assert.Equal("apple", translateViewModel.TargetText); // 断言翻译结果是否正确
        }
    }
}