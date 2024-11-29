namespace PPH.Library.Services;

public interface IMenuNavigationService {
    void NavigateTo(string view, object parameter = null);
}

public static class MenuNavigationConstant {
    public const string TodayWordView = nameof(TodayWordView);
    
    public const string QueryWordView = nameof(QueryWordView);

    public const string FavoriteWordView = nameof(FavoriteWordView);
    
    public const string TranslateView = nameof(TranslateView);
    
    public const string QuizView = nameof(QuizView); 
}