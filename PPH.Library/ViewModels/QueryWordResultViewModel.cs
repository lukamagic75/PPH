using System.Linq.Expressions;
using AvaloniaInfiniteScrolling;
using CommunityToolkit.Mvvm.Input;
using PPH.Library.Models;
using PPH.Library.Services;

namespace PPH.Library.ViewModels;

public class QueryWordResultViewModel : ViewModelBase {
    private readonly IWordStorage _wordStorage;
    
    private readonly IContentNavigationService _contentNavigationService;

    private Expression<Func<ObjectWord, bool>> _where;

    public QueryWordResultViewModel(IWordStorage wordStorage, 
        IContentNavigationService contentNavigationService) {
        _wordStorage = wordStorage;
        _contentNavigationService = contentNavigationService;
        
        WordCollection = new AvaloniaInfiniteScrollCollection<ObjectWord> {
            OnCanLoadMore = () => _canLoadMore,
            OnLoadMore = async () => {
                Status = Loading;
                var wordList = await _wordStorage.GetWordsAsync(
                    _where, WordCollection.Count, PageSize);
                Status = string.Empty;
        
                if (wordList.Count < PageSize) {
                    _canLoadMore = false;
                    Status = NoMoreResult;
                }
                if (WordCollection.Count == 0 && wordList.Count == 0) {
                    Status = NoResult;
                }
        
                return wordList;
            }
        };
        
        ShowWordDetailCommand = new RelayCommand<ObjectWord>(ShowWordDetail);
    }
    
    public override void SetParameter(object parameter) {
        if (parameter is not Expression<Func<ObjectWord, bool>> where) {
            return;
        }

        _where = where;
        _canLoadMore = true;
        WordCollection.Clear();
    }
    
    public IRelayCommand<ObjectWord> ShowWordDetailCommand { get; }

    public void ShowWordDetail(ObjectWord objectWord) {
        _contentNavigationService.NavigateTo(ContentNavigationConstant.DetailView, objectWord);
    }
    
    public AvaloniaInfiniteScrollCollection<ObjectWord> WordCollection { get;  }
    
    private bool _canLoadMore = true;
    
    private string _status;
    public string Status {
        get => _status;
        private set => SetProperty(ref _status, value); 
    }
    
    public const int PageSize = 50;
    
    public const string Loading = "正在载入";
    public const string NoResult = "没有满足条件的结果";
    public const string NoMoreResult = "没有更多结果";
}