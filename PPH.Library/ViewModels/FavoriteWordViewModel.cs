using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using PPH.Library.Models;
using PPH.Library.Services;
using MvvmHelpers;

namespace PPH.Library.ViewModels;

public class FavoriteWordViewModel : ViewModelBase {
    private readonly IFavoriteWordStorage _favoriteStorage;
    private readonly IWordStorage _wordStorage;
    private readonly IContentNavigationService _contentNavigationService;

    public FavoriteWordViewModel(IFavoriteWordStorage favoriteStorage, 
        IWordStorage wordStorage, 
        IContentNavigationService contentNavigationService) {
        _favoriteStorage = favoriteStorage;
        _wordStorage = wordStorage;
        _contentNavigationService = contentNavigationService;

        _favoriteStorage.Updated += FavoriteStorageOnUpdated;
        
        OnInitializedCommand = new AsyncRelayCommand(OnInitializedAsync);
        ShowWordDetailCommand = new RelayCommand<ObjectWord>(ShowWordDetail);
    }
    
    public ObservableRangeCollection<FavoriteWordCombination> FavouriteWordCollection { get; } = new();
    
    public bool IsLoading {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    private bool _isLoading;
    
    public ICommand OnInitializedCommand { get; }
    public async Task OnInitializedAsync() {
        IsLoading = true;

        FavouriteWordCollection.Clear();
        var favoriteList = await _favoriteStorage.GetFavoriteListAsync();

        FavouriteWordCollection.AddRange((await Task.WhenAll(
            favoriteList.Select(p => Task.Run(async () => new FavoriteWordCombination {
                ObjectWord = await _wordStorage.GetWordAsync(p.WordId), 
                Favorite = p
            })))).ToList());

        IsLoading = false;
    }
    
    public IRelayCommand<ObjectWord> ShowWordDetailCommand { get; }

    public void ShowWordDetail(ObjectWord wordObject) {
        _contentNavigationService.NavigateTo(ContentNavigationConstant.DetailView, wordObject);
    }
    
    private async void FavoriteStorageOnUpdated(object sender,
        FavoriteStorageUpdatedEventArgs e) {
        var favorite = e.UpdatedFavorite;

        if (!favorite.IsFavorite) {
            FavouriteWordCollection.Remove(
                FavouriteWordCollection.FirstOrDefault(p =>
                    p.Favorite.WordId == favorite.WordId));
            return;
        }

        var wordFavoriteCombination = new FavoriteWordCombination {
            ObjectWord = await _wordStorage.GetWordAsync(favorite.WordId), 
            Favorite = favorite
        };

        var index = FavouriteWordCollection.IndexOf(
            FavouriteWordCollection.FirstOrDefault(p =>
                p.Favorite.Timestamp < favorite.Timestamp));
        if (index < 0) {
            index = FavouriteWordCollection.Count;
        }

        FavouriteWordCollection.Insert(index, wordFavoriteCombination);
    }
}

public class FavoriteWordCombination {
    public ObjectWord ObjectWord { get; set; }
    
    public FavoriteWord Favorite { get; set; }
}

