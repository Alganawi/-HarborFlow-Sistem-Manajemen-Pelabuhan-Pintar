using HarborFlow.Core.Models;
using System.Threading.Tasks;

namespace HarborFlow.Core.Interfaces
{
    public interface IWeatherService
    {
        Task<WeatherData?> GetWeatherForLocationAsync(double latitude, double longitude);
    }
}
