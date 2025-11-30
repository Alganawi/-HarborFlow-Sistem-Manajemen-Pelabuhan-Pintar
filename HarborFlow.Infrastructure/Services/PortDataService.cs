using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;

namespace HarborFlow.Infrastructure.Services
{
    public class PortDataService : IPortDataService
    {
        public Task<IEnumerable<Port>> GetPortsAsync()
        {
            var ports = new List<Port>
            {
                new Port { Name = "Port of Rotterdam", Country = "Netherlands", Code = "NLRTM", Latitude = 51.9566, Longitude = 4.1200 },
                new Port { Name = "Port of Singapore", Country = "Singapore", Code = "SGSIN", Latitude = 1.2644, Longitude = 103.8400 },
                new Port { Name = "Port of Shanghai", Country = "China", Code = "CNSHA", Latitude = 31.2222, Longitude = 121.4581 },
                new Port { Name = "Port of Los Angeles", Country = "USA", Code = "USLAX", Latitude = 33.7288, Longitude = -118.2620 },
                new Port { Name = "Port of Busan", Country = "South Korea", Code = "KRPUS", Latitude = 35.1028, Longitude = 129.0403 },
                new Port { Name = "Port of Antwerp", Country = "Belgium", Code = "BEANR", Latitude = 51.2194, Longitude = 4.4025 },
                new Port { Name = "Port of Hamburg", Country = "Germany", Code = "DEHAM", Latitude = 53.5488, Longitude = 9.9872 },
                new Port { Name = "Port of Tanjung Priok", Country = "Indonesia", Code = "IDTPP", Latitude = -6.1000, Longitude = 106.8833 },
                new Port { Name = "Port of Tanjung Perak", Country = "Indonesia", Code = "IDSUB", Latitude = -7.1956, Longitude = 112.7333 }
            };

            return Task.FromResult((IEnumerable<Port>)ports);
        }
    }
}
