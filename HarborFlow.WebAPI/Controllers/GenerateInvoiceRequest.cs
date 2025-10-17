namespace HarborFlow.WebAPI.Controllers
{
    public class GenerateInvoiceRequest
    {
        public int ServiceRequestId { get; set; }
        public double TotalAmount { get; set; }
    }
}
