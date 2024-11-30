namespace PPH.UnitTest.ViewModels;

using System.Threading.Tasks;
using PPH.Library.Models;
using PPH.Library.Services;
using PPH.Library.ViewModels;
using FluentAssertions;
using Moq;
using Xunit;


public class WeatherViewModelTests
{
    private readonly Mock<IWeatherService> _weatherServiceMock;
    private readonly WeatherViewModel _viewModel;

    public WeatherViewModelTests()
    {
        _weatherServiceMock = new Mock<IWeatherService>();
        _viewModel = new WeatherViewModel(_weatherServiceMock.Object);
    }

    [Fact]
    public async Task LoadWeatherDataAsync_ShouldUpdateProperties_WhenApiReturnsSuccess() {
        // Arrange
        var weatherResponse = new WeatherResponse {
            Now = new WeatherNow {
                Temp = "15",
                Text = "晴",
                Icon = "100",
                WindDir = "西南风",
                WindSpeed = "9",
                Humidity = "29",
                Precip = "0.0",
                Pressure = "1011",
                Vis = "30",
                Cloud = "8",
                Dew = "-2"
            }
        };
        _weatherServiceMock
            .Setup(s => s.GetCurrentWeatherAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(weatherResponse);

        // Act
        await _viewModel.LoadWeatherDataAsync();

        // Assert
        _viewModel.Temperature.Should().Be("15°C");
        _viewModel.WeatherDescription.Should().Be("晴");
        _viewModel.WindInfo.Should().Be("西南风 | 9 km/h");
        _viewModel.Humidity.Should().Be("29%");
        _viewModel.Precipitation.Should().Be("0.0 mm");
        _viewModel.Pressure.Should().Be("1011 hPa");
        _viewModel.Visibility.Should().Be("30 km");
        _viewModel.CloudCover.Should().Be("8%");
        _viewModel.DewPoint.Should().Be("-2°C");
    }

    [Fact]
    public async Task Load7DaysWeatherAsync_ShouldUpdateDailyWeather_WhenApiReturnsSuccess() {
        // Arrange
        var weatherResponse = new Weather7DaysResponse {
            Daily = new List<WeatherDaily>
            {
                new WeatherDaily { FxDate = "2024-12-01", IconDay = "100", TempMin = "5", TempMax = "15" },
                new WeatherDaily { FxDate = "2024-12-02", IconDay = "101", TempMin = "3", TempMax = "12" }
            }
        };
        _weatherServiceMock
            .Setup(s => s.Get7DaysWeatherAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(weatherResponse);

        // Act
        await _viewModel.Load7DaysWeatherAsync();

        // Assert
        _viewModel.DailyWeather.Should().HaveCount(2);
        _viewModel.DailyWeather[0].FxDate.Should().Be("2024-12-01");
        _viewModel.DailyWeather[0].TempMax.Should().Be("15");
        _viewModel.DailyWeather[0].TempMin.Should().Be("5");
    }

    [Fact]
    public async Task LoadHourlyWeatherAsync_ShouldUpdateHourlyWeather_WhenApiReturnsSuccess() {
        // Arrange
        var hourlyResponse = new HourlyWeatherResponse {
            Hourly = new List<HourlyWeather> {
                new HourlyWeather { FxTime = "2024-12-01T10:00+08:00", Temp = "10", Icon = "100", Text = "晴" },
                new HourlyWeather { FxTime = "2024-12-01T11:00+08:00", Temp = "12", Icon = "150", Text = "多云" }
            }
        };
        _weatherServiceMock
            .Setup(s => s.GetHourlyWeatherAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(hourlyResponse);

        // Act
        await _viewModel.LoadHourlyWeatherAsync();

        // Assert
        _viewModel.HourlyWeather.Should().HaveCount(2);
        _viewModel.HourlyWeather[0].Temp.Should().Be("10");
    }

    [Fact]
    public async Task LoadWeatherIndicesAsync_ShouldUpdateWeatherIndices_WhenApiReturnsSuccess()
    {
        // Arrange
        var indicesResponse = new WeatherIndicesResponse
        {
            Daily = new List<WeatherIndex>
            {
                new WeatherIndex { Text = "适宜运动，天气较好。" },
                new WeatherIndex { Text = "适宜洗车，未来无雨。" }
            }
        };
        _weatherServiceMock
            .Setup(s => s.GetWeatherIndicesAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(indicesResponse);

        // Act
        await _viewModel.LoadWeatherIndicesAsync("101010100");

        // Assert
        _viewModel.WeatherIndices.Should().HaveCount(2);
        _viewModel.WeatherIndices[0].Should().Be("适宜运动，天气较好。");
        _viewModel.WeatherIndices[1].Should().Be("适宜洗车，未来无雨。");
    }

    [Fact]
    public async Task SearchAndLoadWeatherAsync_ShouldUpdateCityNameAndWeather_WhenApiReturnsSuccess() {
        // Arrange
        var citySearchResult = new CitySearchResult {
            Locations = new List<City>
            {
                new City { Name = "上海", Id = "101020100" }
            }
        };
        _weatherServiceMock
            .Setup(s => s.SearchCityAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(citySearchResult);

        var weatherResponse = new WeatherResponse {
            Now = new WeatherNow
            {
                Temp = "20",
                Text = "多云",
                Icon = "101"
            }
        };
        _weatherServiceMock
            .Setup(s => s.GetCurrentWeatherAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(weatherResponse);

        // Act
        _viewModel.SearchCityName = "上海";
        await _viewModel.SearchAndLoadWeatherAsync();

        // Assert
        _viewModel.CityName.Should().Be("上海");
        _viewModel.Temperature.Should().Be("20°C");
        _viewModel.WeatherDescription.Should().Be("多云");
    }
    
    [Fact]
    public async Task AutoLocateAndLoadWeatherAsync_ShouldUpdateCityNameAndWeather_WhenApiReturnsSuccess_ForShenyangShenhe() {
        // Arrange: 模拟 API 响应
        var ipLocationResponse = new IpLocationResponse {
            Status = "success",
            Lat = 41.7968, // 沈阳沈河区纬度
            Lon = 123.4328 // 沈阳沈河区经度
        }; var citySearchResult = new CitySearchResult {
            Locations = new List<City>
            {
                new City { Name = "沈河", Id = "101070101" } 
            }
        }; var weatherResponse = new WeatherResponse {
            Now = new WeatherNow
            {
                Temp = "18",
                Text = "多云",
                Icon = "101"
            }
        };

        // 模拟服务方法的返回值
        _weatherServiceMock
            .Setup(s => s.GetCoordinatesFromIpAsync())
            .ReturnsAsync(ipLocationResponse);
        _weatherServiceMock
            .Setup(s => s.GetCityInfoByCoordinatesAsync(It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync(citySearchResult);
        _weatherServiceMock
            .Setup(s => s.GetCurrentWeatherAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(weatherResponse);

        // Act: 调用方法
        await _viewModel.AutoLocateAndLoadWeatherAsync();

        // Assert: 验证数据是否正确更新
        _viewModel.LocationStatus.Should().Be("定位成功");
        _viewModel.CityName.Should().Be("沈河");
        _viewModel.Temperature.Should().Be("18°C");
        _viewModel.WeatherDescription.Should().Be("多云");

        // 验证服务方法是否被正确调用
        _weatherServiceMock.Verify(s => s.GetCoordinatesFromIpAsync(), Times.Once);
        _weatherServiceMock.Verify(s => s.GetCityInfoByCoordinatesAsync(ipLocationResponse.Lat, ipLocationResponse.Lon), Times.Once);
        _weatherServiceMock.Verify(s => s.GetCurrentWeatherAsync("101070101", It.IsAny<string>()), Times.Once);
    }
}

