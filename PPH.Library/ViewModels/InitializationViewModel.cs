using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using PPH.Library.Services;

namespace PPH.Library.ViewModels;

public class InitializationViewModel : ViewModelBase {
    private readonly IWordStorage _wordStorage;
    private readonly IRootNavigationService _rootNavigationService;
    private readonly IFavoriteWordStorage _favoriteWordStorage;

    public InitializationViewModel(IWordStorage wordStorage, 
        IRootNavigationService rootNavigationService,
        IFavoriteWordStorage favoriteWordStorage) {
        _wordStorage = wordStorage;
        _rootNavigationService = rootNavigationService;
        _favoriteWordStorage = favoriteWordStorage;
        
        OnInitializedCommand = new AsyncRelayCommand(OnInitializedAsync);
    }
    
    public ICommand OnInitializedCommand { get; }

    public async Task OnInitializedAsync() {
        if (!_wordStorage.IsInitialized) {
            await _wordStorage.InitializeAsync();
        }
        
        if (!_favoriteWordStorage.IsInitialized) {
            await _favoriteWordStorage.InitializeAsync();
        }

        await Task.Delay(3000);

        _rootNavigationService.NavigateTo(RootNavigationConstant.MainView);
    }

}

