using Moq;
using PPH.Library.Models;
using PPH.Library.Services;
using PPH.Library.ViewModels;
using System.Linq.Expressions;
using Xunit;

namespace PPH.UnitTest.ViewModels
{
    public class QueryWordViewModelTest
    {
        // 测试 Query 方法
        [Fact]
        public void Query_Default_EnglishFilter()
        {
            // 设置 Mock
            var contentNavigationServiceMock = new Mock<IContentNavigationService>();
            var viewModel = new QueryWordViewModel(contentNavigationServiceMock.Object);

            // 设置过滤器为 "按英文查找"
            viewModel.Filter.Type = FilterType.EnglishWordFilter;
            viewModel.Filter.QueryText = "test";

            // 执行查询
            viewModel.Query();

            // 生成的表达式应该是 (p => p.Word.Contains("test"))
            var parameter = Expression.Parameter(typeof(ObjectWord), "p");
            var expectedExpression = Expression.Call(
                Expression.Property(parameter, nameof(ObjectWord.Word)),
                typeof(string).GetMethod("Contains", new[] { typeof(string) }),
                Expression.Constant("test", typeof(string))
            );
            var lambdaExpression = Expression.Lambda<Func<ObjectWord, bool>>(expectedExpression, parameter);

            // 检查是否调用了 NavigateTo 并传递了正确的查询表达式
            contentNavigationServiceMock.Verify(
                p => p.NavigateTo(ContentNavigationConstant.QueryWordResultView, lambdaExpression),
                Times.Once
            );
        }

        [Fact]
        public void Query_Default_ChineseMeaningFilter()
        {
            // 设置 Mock
            var contentNavigationServiceMock = new Mock<IContentNavigationService>();
            var viewModel = new QueryWordViewModel(contentNavigationServiceMock.Object);

            // 设置过滤器为 "按中文释义查找"
            viewModel.Filter.Type = FilterType.ChineseMeaningFilter;
            viewModel.Filter.QueryText = "测试";

            // 执行查询
            viewModel.Query();

            // 生成的表达式应该是 (p => p.CnMeaning.Contains("测试"))
            var parameter = Expression.Parameter(typeof(ObjectWord), "p");
            var expectedExpression = Expression.Call(
                Expression.Property(parameter, nameof(ObjectWord.CnMeaning)),
                typeof(string).GetMethod("Contains", new[] { typeof(string) }),
                Expression.Constant("测试", typeof(string))
            );
            var lambdaExpression = Expression.Lambda<Func<ObjectWord, bool>>(expectedExpression, parameter);

            // 检查是否调用了 NavigateTo 并传递了正确的查询表达式
            contentNavigationServiceMock.Verify(
                p => p.NavigateTo(ContentNavigationConstant.QueryWordResultView, lambdaExpression),
                Times.Once
            );
        }

        [Fact]
        public void SetParameter_ValidQueryWord()
        {
            // 设置 Mock
            var contentNavigationServiceMock = new Mock<IContentNavigationService>();
            var viewModel = new QueryWordViewModel(contentNavigationServiceMock.Object);

            // 设置传入的 QueryWord 参数
            var queryWord = new QueryWord { Word = "apple", CnMeaning = "苹果" };
            viewModel.SetParameter(queryWord);

            // 断言 Filter 的 QueryText 被正确设置
            Assert.Equal("apple", viewModel.Filter.QueryText);
            Assert.Equal(FilterType.EnglishWordFilter, viewModel.Filter.Type);
        }

        [Fact]
        public void SetParameter_InvalidQueryWord()
        {
            // 设置 Mock
            var contentNavigationServiceMock = new Mock<IContentNavigationService>();
            var viewModel = new QueryWordViewModel(contentNavigationServiceMock.Object);

            // 传入一个无效的参数（不是 QueryWord 类型）
            viewModel.SetParameter("invalid_parameter");

            // 断言 Filter 的 QueryText 不会被修改
            Assert.Equal("请输入想要查询的英文单词或中文释义", viewModel.Filter.QueryText);
        }
    }
}
