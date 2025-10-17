using HarborFlow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HarborFlow.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "FinanceAdmin")]
    public class InvoicesController : ControllerBase
    {
        private readonly HarborService _harborService;

        public InvoicesController(HarborService harborService)
        {
            _harborService = harborService;
        }

        // POST /api/invoices
        [HttpPost]
        public async Task<IActionResult> GenerateInvoice([FromBody] GenerateInvoiceRequest request)
        {
            var invoice = await _harborService.GenerateInvoiceAsync(request.ServiceRequestId, request.TotalAmount);
            if (invoice == null)
            {
                return BadRequest("Failed to generate invoice. The service request may not exist or may already have an invoice.");
            }
            return Ok(invoice);
        }

        // POST /api/invoices/{id}/payments
        [HttpPost("{id}/payments")]
        public async Task<IActionResult> RecordPayment(int id, [FromBody] RecordPaymentRequest request)
        {
            var payment = await _harborService.RecordPaymentAsync(id, request.Amount, request.PaymentMethod);
            if (payment == null)
            {
                return BadRequest("Failed to record payment. The invoice may not exist or may already be paid.");
            }
            return Ok(payment);
        }
    }
}
