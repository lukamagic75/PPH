using System;
using PPH.Library.Services;
using PPH.Library.ViewModels;

namespace PPH.Services;

public class ContentNavigationService : IContentNavigationService {
    public void NavigateTo(string view, object parameter = null) {
        ViewModelBase viewModel = view switch {
            ContentNavigationConstant.DetailView => ServiceLocator.Current
                .DetailViewModel,
            ContentNavigationConstant.QueryWordResultView => ServiceLocator.Current
                .QueryWordResultViewModel,
            _ => throw new Exception("未知的视图。")
        };
        
        if (parameter != null) {
            viewModel.SetParameter(parameter);
        }
        
        ServiceLocator.Current.MainViewModel.PushContent(viewModel);
    }
}