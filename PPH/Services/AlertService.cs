using System.Threading.Tasks;
using PPH.Library.Services;
using Ursa.Controls;

namespace PPH.Services;

public class AlertService : IAlertService {
    public async Task AlertAsync(string title, string message) =>
        await MessageBox.ShowAsync(message, title, button: MessageBoxButton.OK);
}