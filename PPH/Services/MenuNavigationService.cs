using System;
using PPH.Library.Services;
using PPH.Library.ViewModels;

namespace PPH.Services;

public class MenuNavigationService : IMenuNavigationService {
    public void NavigateTo(string view, object parameter = null) {
        ViewModelBase viewModel = view switch {
            MenuNavigationConstant.TodayWordView => ServiceLocator.Current
                .TodayWordViewModel,
            MenuNavigationConstant.TranslateView => ServiceLocator.Current.
                TranslateViewModel,
            MenuNavigationConstant.QueryWordView => ServiceLocator.Current.
                QueryWordViewModel,
            MenuNavigationConstant.FavoriteWordView => ServiceLocator.Current.
                FavoriteWordViewModel,
            MenuNavigationConstant.QuizView => ServiceLocator.Current.
                QuizViewModel,
            MenuNavigationConstant.MemoView => ServiceLocator.Current.
                MemoViewModel,
            MenuNavigationConstant.WeatherView => ServiceLocator.Current.
                WeatherViewModel,
            MenuNavigationConstant.ChatView => ServiceLocator.Current.
                ChatViewModel,
            MenuNavigationConstant.MusicPlayerView => ServiceLocator.Current.
                MusicPlayerViewModel,
            _ => throw new Exception("未知的视图。")
        };

        if (parameter is not null) {
            viewModel.SetParameter(parameter);
        }

        ServiceLocator.Current.MainViewModel.SetMenuAndContent(view, viewModel);
    }
}