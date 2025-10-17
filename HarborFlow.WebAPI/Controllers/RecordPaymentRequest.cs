using System.ComponentModel.DataAnnotations;

namespace HarborFlow.WebAPI.Controllers
{
    public class RecordPaymentRequest
    {
        [Range(0.01, double.MaxValue)]
        public double Amount { get; set; }
        [Required]
        public string PaymentMethod { get; set; } = string.Empty;
    }
}
