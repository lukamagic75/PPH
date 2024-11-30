using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using PPH.Library.Services;

namespace PPH.Library.ViewModels;

public class MainWindowViewModel : ViewModelBase {
    private readonly IWordStorage _wordStorage;
    private readonly IMemoStorage _memoStorage;
    private readonly IRootNavigationService _rootNavigationService;
    private readonly IFavoriteWordStorage _favoriteWordStorage;

    public MainWindowViewModel(IWordStorage wordStorage, 
        IMemoStorage memoStorage,
        IRootNavigationService rootNavigationService,
        IFavoriteWordStorage favoriteWordStorage) {
        _wordStorage = wordStorage;
        _memoStorage = memoStorage;
        _rootNavigationService = rootNavigationService;
        _favoriteWordStorage = favoriteWordStorage;
        
        OnInitializedCommand = new RelayCommand(OnInitialized);
    }

    private ViewModelBase _content;
    
    // 内部提供一个ViewModel
    public ViewModelBase Content {
        get => _content;
        set => SetProperty(ref _content, value);
    }
    
    public ICommand OnInitializedCommand { get; }

    public void OnInitialized() {
        if (!_wordStorage.IsInitialized || !_favoriteWordStorage.IsInitialized || !_memoStorage.IsInitialized) {
            _rootNavigationService.NavigateTo(RootNavigationConstant.InitializationView);
        }
        else {
            _rootNavigationService.NavigateTo(RootNavigationConstant.MainView);
        }
    }
}