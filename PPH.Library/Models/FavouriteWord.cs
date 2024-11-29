using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace PPH.Library.Models;

public class FavoriteWord : ObservableObject {
    [PrimaryKey] public int WordId { get; set; }
    
    private bool _isFavorite;
    public virtual bool IsFavorite {
        get => _isFavorite;
        set => SetProperty(ref _isFavorite, value);
    }
    
    public DateTime Timestamp { get; set; }
}