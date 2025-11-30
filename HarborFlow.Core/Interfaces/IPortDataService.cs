using HarborFlow.Core.Models;

namespace HarborFlow.Core.Interfaces
{
    public interface IPortDataService
    {
        Task<IEnumerable<Port>> GetPortsAsync();
    }
}
