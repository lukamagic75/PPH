using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PPH.Library.Services;

namespace PPH.Library.ViewModels
{
    public partial class ChatViewModel : ViewModelBase
    {
        private readonly IChatService _chatService;

        [ObservableProperty]
        private string askText;

        [ObservableProperty]
        private string responseText;

        [ObservableProperty]
        private string selectedLanguage;

        public string[] Languages { get; set; }

        public ObservableCollection<ChatEntry> ChatEntries { get; } = new();

        public ChatViewModel(IChatService chatService)
        {
            _chatService = chatService;
            AskText = " ";
            ResponseText = " ";
            SelectedLanguage = "中文";
            Languages = new string[] { "中文", "英文" };
        }

        [RelayCommand]
        private async Task Ask()
        {
            if (!string.IsNullOrWhiteSpace(AskText))
            {
                var response = await _chatService.GetAIResponseAsync(AskText);
                ChatEntries.Add(new ChatEntry { InputText = AskText, OutputText = response });
                AskText = string.Empty;
            }
        }

        [RelayCommand]
        private async Task Translate()
        {
            if (!string.IsNullOrWhiteSpace(AskText))
            {
                string skPrompt = $"""
                                   {AskText}

                                   将上面的输入翻译成{SelectedLanguage}，无需任何其他内容
                                   """;

                var response = await _chatService.GetAIResponseAsync(skPrompt);
                ChatEntries.Add(new ChatEntry { InputText = AskText, OutputText = response });
                AskText = string.Empty;
            }
        }
    }

    public class ChatEntry
    {
        public string InputText { get; set; }
        public string OutputText { get; set; }
    }
}
