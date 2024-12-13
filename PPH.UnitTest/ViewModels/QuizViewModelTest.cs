using PPH.Library.Services;
using PPH.Library.ViewModels;
using PPH.UnitTest.Helpers;
using Moq;

namespace PPH.UnitTest.ViewModels;

public class QuizViewModelTest {
    [Fact]
    public async Task Update_Default() {
        var wordStorage = await WordStorageHelper.GetInitializedWordStorage();
        var contentNavigationServiceMock = new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;
        var wordQuizViewModel = new QuizViewModel(wordStorage, mockContentNavigationService);

        await Task.Delay(1000);
        
        Assert.Equal(4, wordQuizViewModel.QuizOptions.Count);
        var oldWord = wordQuizViewModel.CorrectWord;
        Assert.NotNull(oldWord);
        
        wordQuizViewModel.Update();
        
        await Task.Delay(1000);
        Assert.NotEqual(oldWord, wordQuizViewModel.CorrectWord);
        await wordStorage.CloseAsync();
    }
    
    [Fact]
    public async Task ShowDetailCommandFunction_Default() {
        var wordStorage = await WordStorageHelper.GetInitializedWordStorage();
        var contentNavigationServiceMock = new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;
        var wordQuizViewModel = new QuizViewModel(wordStorage, mockContentNavigationService);

        await Task.Delay(1000);
        wordQuizViewModel.ShowDetail();
        contentNavigationServiceMock.Verify(
            p => p.NavigateTo(ContentNavigationConstant.DetailView, wordQuizViewModel.CorrectWord),
            Times.Once);
        await wordStorage.CloseAsync();
    }
}