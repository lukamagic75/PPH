using System.Linq.Expressions;
using PPH.Library.Models;
using PPH.Library.Services;
using PPH.Library.ViewModels;
using PPH.UnitTest.Helpers;
using Moq;

namespace PPH.UnitTest.ViewModels;

public class QueryWordResultViewModelTest : IDisposable
{
    public QueryWordResultViewModelTest() =>
        WordStorageHelper.RemoveDatabaseFile();

    public void Dispose() => WordStorageHelper.RemoveDatabaseFile();
    
    [Fact]
    public async Task WordCollection_Default() {
        var where = Expression.Lambda<Func<ObjectWord, bool>>(
            Expression.Constant(true),
            Expression.Parameter(typeof(ObjectWord), "p"));

        var wordStorage =
            await WordStorageHelper.GetInitializedWordStorage();
        var resultViewModel = new QueryWordResultViewModel(wordStorage, null);
        resultViewModel.SetParameter(where);

        var statusList = new List<string>();
        resultViewModel.PropertyChanged += (sender, args) => {
            if (args.PropertyName == nameof(resultViewModel.Status)) {
                statusList.Add(resultViewModel.Status);
            }
        };

        Assert.Empty(resultViewModel.WordCollection);
        await resultViewModel.WordCollection.LoadMoreAsync();
        Assert.Equal(50, resultViewModel.WordCollection.Count);
        Assert.Equal("steer",
            resultViewModel.WordCollection.Last().Word);
        Assert.True(resultViewModel.WordCollection.CanLoadMore);
        Assert.Equal(2, statusList.Count);
        Assert.Equal(QueryWordResultViewModel.Loading, statusList[0]);
        Assert.Equal("", statusList[1]);

        var wordCollectionChanged = false;
        resultViewModel.WordCollection.CollectionChanged += (sender, args) => wordCollectionChanged = true;
        await resultViewModel.WordCollection.LoadMoreAsync();
        Assert.True(wordCollectionChanged);
        Assert.Equal(100, resultViewModel.WordCollection.Count);
        Assert.Equal("tag",
            resultViewModel.WordCollection[99].Word);

        await wordStorage.CloseAsync();
    }
    
    [Fact]
    public void ShowDetail_Default() {
        var contentNavigationServiceMock =
            new Mock<IContentNavigationService>();
        var mockContentNavigationService =
            contentNavigationServiceMock.Object;

        var wordToTap = new ObjectWord();
        var resultViewModel =
            new QueryWordResultViewModel(null, mockContentNavigationService);
        resultViewModel.ShowWordDetail(wordToTap);
        contentNavigationServiceMock.Verify(
            p => p.NavigateTo(ContentNavigationConstant.DetailView, wordToTap), Times.Once);
    }
}