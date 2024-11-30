namespace PPH.Library.Services;

using PPH.Library.Models;
using System.Threading.Tasks;



public interface IWeatherService
{
    Task<WeatherResponse> GetCurrentWeatherAsync(string location, string apiKey);
    
    Task<Weather7DaysResponse> Get7DaysWeatherAsync(string location, string apiKey);
    
    Task<CitySearchResult> SearchCityAsync(string query, string apiKey);

    Task<IpLocationResponse> GetCoordinatesFromIpAsync();

    Task<CitySearchResult> GetCityInfoByCoordinatesAsync(double latitude, double longitude);

    Task<City> AutoLocateCityAsync();
    
    Task<HourlyWeatherResponse> GetHourlyWeatherAsync(string location, string apiKey);
    
    Task<WeatherIndicesResponse> GetWeatherIndicesAsync(string location, string apiKey);
}