using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using PPH.Library.Models;
using PPH.Library.Services;
using MvvmHelpers;

namespace PPH.Library.ViewModels;

public class QuizViewModel : ViewModelBase {
    private readonly IWordStorage _wordStorage;
    private IContentNavigationService _contentNavigationService;

    public QuizViewModel(IWordStorage wordStorage, 
        IContentNavigationService contentNavigationService) {
        _wordStorage = wordStorage;
        _contentNavigationService = contentNavigationService;
        
        UpdateCommand = new RelayCommand(Update);
        CommitCommand = new RelayCommand(Commit);
        RadioCheckedCommand = new RelayCommand<ObjectWord>(RadioChecked);
        SelectModeCommand = new RelayCommand<string>(SelectMode);
        ShowDetailCommand = new RelayCommand(ShowDetail);
        
        Update();
    }
    
    private ObjectWord _correctWord;
    public ObjectWord CorrectWord {
        get => _correctWord;
        set => SetProperty(ref _correctWord, value);
    }
    
    
    public ObservableRangeCollection<ObjectWord> QuizOptions { get; } = new();

    public static ObservableRangeCollection<string> QuizModes { get; } 
        = ["解释选择", "中文选词"];
    
    private string _selectedMode = QuizModes[0]; 
    public string SelectedMode {
        get => _selectedMode;
        private set => SetProperty(ref _selectedMode, value);
    }
    
    public ICommand SelectModeCommand { get; }
    private void SelectMode(string mode) {
        if (mode == QuizModes[0] || mode == QuizModes[1]) {
            SelectedMode = mode;
            Update();
        }
    }

    private string _resultText;
    public string ResultText {
        get => _resultText;
        set => SetProperty(ref _resultText, value);
    }
    
    
    private bool _hasAnswered; //已提交答案
    public bool HasAnswered {
        get => _hasAnswered;
        set => SetProperty(ref _hasAnswered, value);
    }
    
    private bool _hasSelected; //已选择某一选项
    public bool HasSelected {
        get => _hasSelected;
        set => SetProperty(ref _hasSelected, value);
    }
    
    private ObjectWord _selectedOption;
    public ObjectWord SelectedOption {
        get => _selectedOption;
        set => SetProperty(ref _selectedOption, value);
    }
    
    public bool IsLoading {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    private bool _isLoading;
    
    // 切换到下题
    public ICommand UpdateCommand { get; }
    public void Update() {
        // 初始化时也需要调用
        Task.Run(async () => {
            IsLoading = true;
            HasSelected = false;
            HasAnswered = false;
        
            CorrectWord = await _wordStorage.GetRandomWordAsync();
        
            QuizOptions.Clear();
            var wordList = await _wordStorage.GetWordQuizOptionsAsync(_correctWord);
            QuizOptions.AddRange(wordList);
        
            IsLoading = false;
        });
    }
    
    // 选中某个选项时触发
    public ICommand RadioCheckedCommand { get; }
    private void RadioChecked(ObjectWord selectedWordObject) {
        HasSelected = true;
        SelectedOption = selectedWordObject;
    }
    
    
    public ICommand CommitCommand { get; }
    private void Commit() {
        if (SelectedOption.Word == CorrectWord.Word) {
            ResultText = "恭喜您回答正确！";
        }
        else {
            ResultText = "很遗憾，回答错误啦~";
        }
        HasAnswered = true;
    }

    // 跳转至单词详情页
    public ICommand ShowDetailCommand { get; }
    public void ShowDetail() {
        _contentNavigationService.NavigateTo(
            ContentNavigationConstant.DetailView, CorrectWord);
    }

}