using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Xunit;
using PPH.Library.Services;
using PPH.Library.Models;

public class BingImageServiceTests
{
    private readonly Mock<IAlertService> _alertServiceMock;
    private readonly Mock<ITodayImageStorage> _todayImageStorageMock;
    private readonly HttpClient _httpClient;
    private readonly BingImageService _bingImageService;

    public BingImageServiceTests()
    {
        _alertServiceMock = new Mock<IAlertService>();
        _todayImageStorageMock = new Mock<ITodayImageStorage>();
        
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"images\":[{\"startdate\":\"20231010\",\"url\":\"/image.png\",\"copyright\":\"Copyright Text\",\"title\":\"Image Title\"}]}")
            });

        _httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://www.bing.com/")
        };

        _bingImageService = new BingImageService(_alertServiceMock.Object, _todayImageStorageMock.Object);
    }

    [Fact]
    public async Task GetTodayImageAsync_ReturnsImage()
    {
        // Arrange
        var expectedImage = new TodayImage { FullStartDate = "20231010" };
        _todayImageStorageMock
            .Setup(storage => storage.GetTodayImageAsync(It.IsAny<bool>()))
            .ReturnsAsync(expectedImage);

        // Act
        var result = await _bingImageService.GetTodayImageAsync();

        // Assert
        Assert.Equal(expectedImage.FullStartDate, result.FullStartDate);
    }

    [Fact]
    public async Task CheckUpdateAsync_ReturnsHasUpdateFalse_WhenNotExpired()
    {
        // Arrange
        var todayImage = new TodayImage
        {
            ExpiresAt = DateTime.Now.AddHours(1)
        };
        
        _todayImageStorageMock
            .Setup(storage => storage.GetTodayImageAsync(It.IsAny<bool>()))
            .ReturnsAsync(todayImage);

        // Act
        var result = await _bingImageService.CheckUpdateAsync();

        // Assert
        Assert.False(result.HasUpdate);
    }

    [Fact]
    public async Task CheckUpdateAsync_ReturnsHasUpdateTrue_WhenUpdated()
    {
        // Arrange
        var todayImage = new TodayImage
        {
            ExpiresAt = DateTime.Now.AddHours(-1),
            FullStartDate = "20230910"
        };

        _todayImageStorageMock
            .Setup(storage => storage.GetTodayImageAsync(It.IsAny<bool>()))
            .ReturnsAsync(todayImage);

        _todayImageStorageMock
            .Setup(storage => storage.SaveTodayImageAsync(It.IsAny<TodayImage>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _bingImageService.CheckUpdateAsync();

        // Assert
        Assert.True(result.HasUpdate);
        Assert.NotNull(result.TodayImage);
    }

    // Add more tests for GetRandomImageAsync and other scenarios...
}
