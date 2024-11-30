using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using PPH.Library.Services;

namespace PPH.Library.ViewModels;

public class MainViewModel : ViewModelBase{
    private readonly IMenuNavigationService _menuNavigationService;
    
    public MainViewModel(IMenuNavigationService menuNavigationService) {
        _menuNavigationService = menuNavigationService;
        
        OpenPaneCommand = new RelayCommand(OpenPane);
        ClosePaneCommand = new RelayCommand(ClosePane);
        GoBackCommand = new RelayCommand(GoBack);
        OnMenuTappedCommand = new RelayCommand(OnMenuTapped);
    }
    
    private string _title = "每日单词，不止单词";
    public string Title {
        get => _title;
        private set => SetProperty(ref _title, value);
    }
    
    public ObservableCollection<ViewModelBase> ContentStack { get; } = [];
    // 内部提供一个ViewModel
    private ViewModelBase _content;
    public ViewModelBase Content {
        get => _content;
        private set => SetProperty(ref _content, value);
    }
    
    public void PushContent(ViewModelBase content) {
        ContentStack.Insert(0, Content = content); //同时完成Content赋值和ViewModel入栈的操作
    }
    public void SetMenuAndContent(string view, ViewModelBase content) {
        ContentStack.Clear();
        PushContent(content);
        // 改变菜单项的值
        SelectedMenuItem =
            MenuItem.MenuItems.FirstOrDefault(p => p.View == view);
        Title = SelectedMenuItem.Name;
        IsPaneOpen = false;
    }
    
    private MenuItem _selectedMenuItem;
    public MenuItem SelectedMenuItem {
        get => _selectedMenuItem;
        set => SetProperty(ref _selectedMenuItem, value);
    }
    
    public ICommand OnMenuTappedCommand { get; }

    public void OnMenuTapped() {
        if (SelectedMenuItem is null) {
            return;
        }
        _menuNavigationService.NavigateTo(SelectedMenuItem.View);
    }
    
    private bool _isPaneOpen;
    public bool IsPaneOpen {
        get => _isPaneOpen;
        private set => SetProperty(ref _isPaneOpen, value);
    }
    public ICommand OpenPaneCommand { get; }

    public void OpenPane() => IsPaneOpen = true;

    public ICommand ClosePaneCommand { get; }

    public void ClosePane() => IsPaneOpen = false;
    
    // 返回上一个页面
    public ICommand GoBackCommand { get; }
    public void GoBack() {
        // 如果当前栈中只有这一个页面，则不能再后退
        if (ContentStack.Count <= 1) {
            return;
        }
        ContentStack.RemoveAt(0);
        Content = ContentStack[0];
    }
}

public class MenuItem {
    public string View { get; private init; }
    public string Name { get; private init; }
    
    private MenuItem() { }
    

    private static MenuItem TodayWordView =>
        new() { Name = "今日单词推荐", View = MenuNavigationConstant.TodayWordView };
    
    
    private static MenuItem TranslateView =>
        new() { Name = "文本翻译", View = MenuNavigationConstant.TranslateView };
    
    private static MenuItem QueryWordView =>
        new() { Name = "单词查找", View = MenuNavigationConstant.QueryWordView };

    private static MenuItem FavoriteWordView =>
        new() { Name = "单词收藏", View = MenuNavigationConstant.FavoriteWordView };
    
    private static MenuItem QuizView =>
        new() { Name = "单词测验", View = MenuNavigationConstant.QuizView };
    
    private static MenuItem MemoView =>
        new() { Name = "备忘录", View = MenuNavigationConstant.MemoView };
    
    private static MenuItem WeatherView => 
        new() { Name = "天气预报", View = MenuNavigationConstant.WeatherView };
    
    public static IEnumerable<MenuItem> MenuItems { get; } = [
        TodayWordView, TranslateView, 
        QueryWordView, FavoriteWordView, 
        QuizView, MemoView,
        WeatherView
    ];
}