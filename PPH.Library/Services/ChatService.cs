using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net.Http;
using System.Threading.Tasks;

namespace PPH.Library.Services
{
    public class ChatService: IChatService
    {
        private readonly string _apiKey = "sk-YZ8iticP3niRryqTF16fF03d8e9c4b049bF7BcAa4dFf69Af";
        private readonly string _baseUrl = "https://api.bltcy.ai";

        public async Task<string> GetAIResponseAsync(string userMessage)
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest("v1/chat/completions", Method.Post);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Authorization", $"Bearer {_apiKey}");
            request.AddHeader("Content-Type", "application/json");

            var body = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful assistant." },
                    new { role = "user", content = userMessage }
                }
            };

            request.AddJsonBody(body);

            var response = await client.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                // 解析JSON响应
                var jsonResponse = JObject.Parse(response.Content);
                var messageContent = jsonResponse["choices"]?[0]?["message"]?["content"]?.ToString();

                return messageContent ?? "无法获取翻译内容";
            }
            else
            {
                throw new HttpRequestException($"Error calling AI API: {response.ErrorMessage}");
            }
        }

        public async Task<string> TranslateTextAsync(string text, string language)
        {
            string prompt = $"Translate the following text to {language}: {text}";
            return await GetAIResponseAsync(prompt);
        }
    }
}