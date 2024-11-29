using CommunityToolkit.Mvvm.ComponentModel;

namespace PPH.Library.ViewModels;

public abstract class ViewModelBase : ObservableObject {
    public virtual void SetParameter(object parameter) { }
}