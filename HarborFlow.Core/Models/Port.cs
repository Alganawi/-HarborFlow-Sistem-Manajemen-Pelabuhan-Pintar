using System.Text.Json.Serialization;

namespace HarborFlow.Core.Models
{
    public class Port
    {
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Country { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
