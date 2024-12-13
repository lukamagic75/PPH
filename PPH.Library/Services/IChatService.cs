using System.Collections.Generic;
using System.Threading.Tasks;

namespace PPH.Library.Services
{
    public interface IChatService
    {
        Task<string> GetAIResponseAsync(string userMessage);
        Task<string> TranslateTextAsync(string text, string language);
    }
}