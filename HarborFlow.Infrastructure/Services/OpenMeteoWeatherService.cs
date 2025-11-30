using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace HarborFlow.Infrastructure.Services
{
    public class OpenMeteoWeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;

        public OpenMeteoWeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<WeatherData?> GetWeatherForLocationAsync(double latitude, double longitude)
        {
            try
            {
                var url = $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&current_weather=true";
                var response = await _httpClient.GetFromJsonAsync<OpenMeteoResponse>(url);
                return response?.CurrentWeather;
            }
            catch (Exception ex)
            {
                // Log error here if logging service is available
                Console.WriteLine($"Error fetching weather data: {ex.Message}");
                return null;
            }
        }
    }
}
