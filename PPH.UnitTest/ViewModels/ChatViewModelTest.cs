using System.Threading.Tasks;
using Xunit;
using Moq;
using PPH.Library.Services;
using PPH.Library.ViewModels;

public class ChatViewModelTests
{
    private readonly Mock<IChatService> _mockChatService;
    private readonly ChatViewModel _viewModel;

    public ChatViewModelTests()
    {
        _mockChatService = new Mock<IChatService>();
        _viewModel = new ChatViewModel(_mockChatService.Object);
    }

    [Fact]
    public async Task AskCommand_ShouldAddChatEntry_WhenAskTextIsNotEmpty()
    {
        // Arrange
        var inputText = "Hello";
        var responseText = "Hi there!";
        _mockChatService.Setup(s => s.GetAIResponseAsync(inputText))
                        .ReturnsAsync(responseText);

        _viewModel.AskText = inputText;

        // Act
        await _viewModel.AskCommand.ExecuteAsync(null);

        // Assert
        Assert.Single(_viewModel.ChatEntries);
        Assert.Equal(inputText, _viewModel.ChatEntries[0].InputText);
        Assert.Equal(responseText, _viewModel.ChatEntries[0].OutputText);
        Assert.Equal(string.Empty, _viewModel.AskText);
    }

    [Fact]
    public async Task TranslateCommand_ShouldAddChatEntry_WhenAskTextIsNotEmpty()
    {
        // Arrange
        var inputText = "Apple";
        var selectedLanguage = "中文";
        var translation = "苹果";
        var skPrompt = $"{inputText}\n\n将上面的输入翻译成{selectedLanguage}，无需任何其他内容";

        _mockChatService.Setup(s => s.GetAIResponseAsync(It.Is<string>(s => s.Contains(inputText) && s.Contains(selectedLanguage))))
                        .ReturnsAsync(translation);

        _viewModel.AskText = inputText;
        _viewModel.SelectedLanguage = selectedLanguage;

        // Act
        await _viewModel.TranslateCommand.ExecuteAsync(null);

        // Assert
        Assert.Single(_viewModel.ChatEntries);
        Assert.Equal(inputText, _viewModel.ChatEntries[0].InputText);
        Assert.Equal(translation, _viewModel.ChatEntries[0].OutputText);
        Assert.Equal(string.Empty, _viewModel.AskText);
    }
}
