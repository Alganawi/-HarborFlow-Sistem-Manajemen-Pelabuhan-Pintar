using System.Text.Json.Serialization;

namespace HarborFlow.Core.Models
{
    public class WeatherData
    {
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; }

        [JsonPropertyName("windspeed")]
        public double WindSpeed { get; set; }

        [JsonPropertyName("winddirection")]
        public double WindDirection { get; set; }

        [JsonPropertyName("weathercode")]
        public int WeatherCode { get; set; }

        [JsonPropertyName("time")]
        public string Time { get; set; } = string.Empty;
    }

    public class OpenMeteoResponse
    {
        [JsonPropertyName("current_weather")]
        public WeatherData CurrentWeather { get; set; } = new();
    }
}
