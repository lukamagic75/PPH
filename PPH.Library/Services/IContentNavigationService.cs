namespace PPH.Library.Services;

public interface IContentNavigationService {
    void NavigateTo(string view, object parameter = null);
}

public static class ContentNavigationConstant {
    public const string DetailView = nameof(DetailView); // 有收藏按钮
    
    public const string MottoDetailView = nameof(MottoDetailView);

    public const string QueryWordResultView = nameof(QueryWordResultView);

    
}