using System.ComponentModel.DataAnnotations;

namespace HarborFlow.WebAPI.Controllers
{
    public class CreateVesselRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string ImoNumber { get; set; } = string.Empty;
        [Required]
        public string Type { get; set; } = string.Empty;
        public double Capacity { get; set; }
    }
}
