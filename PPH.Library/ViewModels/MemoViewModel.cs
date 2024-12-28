using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using PPH.Library.Helpers;
using PPH.Library.Models;
using PPH.Library.Services;

namespace PPH.Library.ViewModels;

public class MemoViewModel : ViewModelBase
{
    private readonly IMemoStorage _memoStorage;
    private DateTime _selectedDate;
    private string _newMemoContent;
    private MemoObject _selectedMemo;

    public MemoViewModel(IMemoStorage memoStorage)
    {
        _memoStorage = memoStorage;
        MemoList = new ObservableCollection<MemoObject>();

          
        _selectedDate = DateTime.Now;
    }

    public DateTime SelectedDate {
        get => _selectedDate;
        set {
        }
    }

    public string NewMemoContent {
        get => _newMemoContent;
        set {
            if (_newMemoContent != value)
            {
                _newMemoContent = value;
                OnPropertyChanged();
            }
        }
    }

    public ObservableCollection<MemoObject> MemoList { get; }
    

    
    
    
    public MemoObject SelectedMemo
    {
        get => _selectedMemo;
        set
        {
            if (_selectedMemo != value)
            {
                _selectedMemo = value;
                OnPropertyChanged();
                if (_selectedMemo != null)
                {
                    // 将选中的备忘录内容加载到输入框
                    NewMemoContent = _selectedMemo.Content;
                }
            }
        }
    }
}