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

        public ChatViewModel(IChatService chatService)
        {
            _chatService = chatService;
            AskText = " ";
            ResponseText = " ";
            SelectedLanguage = " ";
            Languages = new string[] { "中文", "英文" };
        }

        [RelayCommand]
        private async Task Ask()
        {
            if (!string.IsNullOrEmpty(ResponseText))
            {
                ResponseText = "";
            }

            // 获取完整的响应并更新ResponseText
            ResponseText = await _chatService.GetAIResponseAsync(AskText);
        }

        [RelayCommand]
        private async Task Translate()
        {
            string skPrompt = $"""
                               {AskText}

                               将上面的输入翻译成{SelectedLanguage}，无需任何其他内容
                               """;

            if (!string.IsNullOrEmpty(ResponseText))
            {
                ResponseText = "";
            }

            // 获取翻译结果并更新ResponseText
            ResponseText = await _chatService.GetAIResponseAsync(skPrompt);
        }
    }
}