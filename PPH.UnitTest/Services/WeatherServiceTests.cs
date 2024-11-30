namespace PPH.UnitTest.Services;

using System.Net;
using System.Text.Json;
using PPH.Library.Models;
using PPH.Library.Services;
using FluentAssertions;
using Moq;
using Moq.Protected;

public class WeatherServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly WeatherService _weatherService;

    public WeatherServiceTests()
    {
        // 初始化 Mock HttpMessageHandler
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://api.qweather.com/")
        };
        _weatherService = new WeatherService();
    }

    [Fact]
    public async Task GetCurrentWeatherAsync_ShouldReturnWeatherResponse_WhenApiReturnsSuccess()
    {
        // Arrange: 模拟 API 返回 JSON 数据
        var sampleResponse = new WeatherResponse {
            Now = new WeatherNow {
                Temp = "16",
                Text = "晴",
                Icon = "100"
            }
        };

        var responseMessage = new HttpResponseMessage {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(sampleResponse))
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act: 调用方法
        var result = await _weatherService.GetCurrentWeatherAsync("101010100", "2ae6633866264232a41d4f9f216dc61b");

        // Assert: 验证结果
        result.Should().NotBeNull();
        result.Now.Temp.Should().Be("16");
        result.Now.Text.Should().Be("晴");
        result.Now.Icon.Should().Be("100");
    }

    [Fact]
    public async Task Get7DaysWeatherAsync_ShouldReturnWeather7DaysResponse_WhenApiReturnsSuccess() {
        // Arrange: 模拟 API 返回 JSON 数据
        var sampleResponse = new Weather7DaysResponse {
            Daily = new List<WeatherDaily> {
                new WeatherDaily { FxDate = "2024-11-30", IconDay = "100", TempMin = "0", TempMax = "15" },
                new WeatherDaily { FxDate = "2024-12-01", IconDay = "101", TempMin = "-1", TempMax = "12" }
            }
        };

        var responseMessage = new HttpResponseMessage {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(sampleResponse))
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act: 调用方法
        var result = await _weatherService.Get7DaysWeatherAsync("101010100", "2ae6633866264232a41d4f9f216dc61b");

        // Assert: 验证结果
        result.Should().NotBeNull();
        result.Daily.Should().HaveCount(7);
        result.Daily[0].FxDate.Should().Be("2024-11-30");
        result.Daily[0].IconDay.Should().Be("100");
        result.Daily[0].TempMin.Should().Be("0");
        result.Daily[0].TempMax.Should().Be("15");
    }

    [Fact]
    public async Task SearchCityAsync_ShouldReturnCitySearchResult_WhenApiReturnsSuccess() {
        // Arrange: 模拟 API 返回 JSON 数据
        var sampleResponse = new CitySearchResult {
            Locations = new List<City> {
                new City { Name = "北京", Id = "101010100" }
            }
        };

        var responseMessage = new HttpResponseMessage {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(sampleResponse))
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act: 调用方法
        var result = await _weatherService.SearchCityAsync("北京", "2ae6633866264232a41d4f9f216dc61b");

        // Assert: 验证结果
        result.Should().NotBeNull();
        result.Locations.Should().HaveCount(10);
        result.Locations[0].Name.Should().Be("北京");
        result.Locations[0].Id.Should().Be("101010100");
    }

    [Fact]
    public async Task GetCoordinatesFromIpAsync_ShouldReturnIpLocationResponse_WhenApiReturnsSuccess() {
        // Arrange: 模拟 API 返回 JSON 数据
        var sampleResponse = new IpLocationResponse {
            Status = "success",
            Lat = 41.8486,
            Lon = -87.6288
        };

        var responseMessage = new HttpResponseMessage {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(sampleResponse))
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act: 调用方法
        var result = await _weatherService.GetCoordinatesFromIpAsync();

        // Assert: 验证结果
        result.Should().NotBeNull();
        result.Status.Should().Be("success");
        result.Lat.Should().Be(41.8486);
        result.Lon.Should().Be(-87.6288);
    }
    
    [Fact]
    public async Task GetHourlyWeatherAsync_ShouldReturnHourlyWeatherResponse_WhenApiReturnsSuccess()
    {
        // Arrange: 模拟 API 返回 JSON 数据
        var sampleResponse = new HourlyWeatherResponse {
            Hourly = new List<HourlyWeather>
            {
                new HourlyWeather { FxTime = "2024-11-30T13:00+08:00", Temp = "14", Icon = "100", Text = "晴" },
                new HourlyWeather { FxTime = "2024-11-30T14:00+08:00", Temp = "9", Icon = "150", Text = "多云" }
            }
        };

        var responseMessage = new HttpResponseMessage {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(sampleResponse))
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act: 调用方法
        var result = await _weatherService.GetHourlyWeatherAsync("101010100", "2ae6633866264232a41d4f9f216dc61b");

        // Assert: 验证结果
        result.Should().NotBeNull();
        result.Hourly.Should().HaveCount(24);
        result.Hourly[0].FxTime.Should().Be("2024-11-30T13:00+08:00");
        result.Hourly[0].Temp.Should().Be("14");
        result.Hourly[0].Icon.Should().Be("100");
        result.Hourly[0].Text.Should().Be("晴");
    }

    [Fact]
    public async Task GetCityInfoByCoordinatesAsync_ShouldReturnCitySearchResult_WhenApiReturnsSuccess()
    {
        // Arrange: 模拟 API 返回 JSON 数据
        var sampleResponse = new CitySearchResult {
            Locations = new List<City>
            {
                new City { Name = "东城", Id = "101011600", Latitude = "39.91755", Longitude = "116.41876" }
            }
        };

        var responseMessage = new HttpResponseMessage {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(sampleResponse))
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act: 调用方法
        var result = await _weatherService.GetCityInfoByCoordinatesAsync(39.91755, 116.41876);

        // Assert: 验证结果
        result.Should().NotBeNull();
        result.Locations.Should().HaveCount(1);
        result.Locations[0].Name.Should().Be("东城");
        result.Locations[0].Id.Should().Be("101011600");
        result.Locations[0].Latitude.Should().Be("39.91755");
        result.Locations[0].Longitude.Should().Be("116.41876");
    }
}    