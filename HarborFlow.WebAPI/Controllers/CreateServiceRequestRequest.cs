using System.ComponentModel.DataAnnotations;

namespace HarborFlow.WebAPI.Controllers
{
    public class CreateServiceRequestRequest
    {
        [Range(1, int.MaxValue)]
        public int VesselId { get; set; }
        public string ServiceType { get; set; } = string.Empty;
    }
}
