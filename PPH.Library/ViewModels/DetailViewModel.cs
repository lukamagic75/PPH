using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using PPH.Library.Models;
using PPH.Library.Services;

namespace PPH.Library.ViewModels;

public class DetailViewModel : ViewModelBase {
    private readonly IMenuNavigationService _menuNavigationService;
    private readonly IFavoriteWordStorage _wordFavoriteStorage;

    public DetailViewModel(IMenuNavigationService menuNavigationService,
        IFavoriteWordStorage wordFavoriteStorage) {
        _menuNavigationService = menuNavigationService;
        _wordFavoriteStorage = wordFavoriteStorage;
        
        OnLoadedCommand = new AsyncRelayCommand(OnLoadedAsync);
        FavoriteSwitchCommand = new AsyncRelayCommand(FavoriteSwitchClickedAsync);
        QueryCommand = new RelayCommand(Query);
    }
    
    private ObjectWord _currentWord;
    public ObjectWord CurrentWord {
        get => _currentWord;
        set => SetProperty(ref _currentWord, value);
    }

    public bool CanShowPhrase => _currentWord.Phrase is { Length: > 0 };
    
    public bool CanShowEtyma => _currentWord.Etyma is { Length: > 0 };

    public override void SetParameter(object parameter) {
        CurrentWord = parameter as ObjectWord;
    }
    
    public FavoriteWord Favorite {
        get => _favorite;
        set => SetProperty(ref _favorite, value);
    }
    private FavoriteWord _favorite;
    
    public bool IsLoading {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    private bool _isLoading;
    
    public ICommand OnLoadedCommand { get; }
    public async Task OnLoadedAsync() {
        IsLoading = true;
        var favorite = await _wordFavoriteStorage.GetFavoriteAsync(CurrentWord.Id);
        if (favorite == null) {
            favorite = new FavoriteWord {
                WordId = CurrentWord.Id
            };
        }
        Favorite = favorite;
        IsLoading = false;
    }
    
    public ICommand FavoriteSwitchCommand { get; }

    public async Task FavoriteSwitchClickedAsync() {
        IsLoading = true;
        await _wordFavoriteStorage.SaveFavoriteAsync(Favorite);
        IsLoading = false;
    }
    
    public ICommand QueryCommand { get; }
    public void Query() {
        _menuNavigationService.NavigateTo(MenuNavigationConstant.QueryWordView, 
            new QueryWord {
                Word = CurrentWord.Word, CnMeaning = CurrentWord.CnMeaning
            });
    }

}