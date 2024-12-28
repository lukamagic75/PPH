using Moq;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace PPH.Library.Services.Tests
{
    public class ChatServiceTests
    {
        private readonly Mock<IRestClient> _restClientMock;
        private readonly ChatService _chatService;

        public ChatServiceTests()
        {
            _restClientMock = new Mock<IRestClient>();
            _chatService = new ChatService();
        }

        [Fact]
        public async Task GetAIResponseAsync_ShouldReturnExpectedResponse()
        {
            // Arrange
            string userMessage = "Hello, AI!";
            string expectedResponse = "Hello, User!";
            var jsonResponse = new JObject
            {
                ["choices"] = new JArray
                {
                    new JObject
                    {
                        ["message"] = new JObject
                        {
                            ["content"] = expectedResponse
                        }
                    }
                }
            };

            var restResponse = new RestResponse
            {
                StatusCode = HttpStatusCode.OK,
                Content = jsonResponse.ToString()
            };

            _restClientMock
                .Setup(client => client.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(restResponse);

            // Act
            var response = await _chatService.GetAIResponseAsync(userMessage);

            // Assert
            Assert.Equal(expectedResponse, response);
        }

        [Fact]
        public async Task GetAIResponseAsync_ShouldThrowHttpRequestException_OnErrorResponse()
        {
            // Arrange
            string userMessage = "Hello, AI!";
            var restResponse = new RestResponse
            {
                StatusCode = HttpStatusCode.BadRequest,
                ErrorMessage = "Bad Request"
            };

            _restClientMock
                .Setup(client => client.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(restResponse);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _chatService.GetAIResponseAsync(userMessage));
        }
    }
}