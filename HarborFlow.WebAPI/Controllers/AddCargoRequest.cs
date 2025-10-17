using System.ComponentModel.DataAnnotations;

namespace HarborFlow.WebAPI.Controllers
{
    public class AddCargoRequest
    {
        [Required]
        public string Description { get; set; } = string.Empty;
        public double Weight { get; set; }
        public bool IsHazardous { get; set; }
    }
}
