using HarborFlow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HarborFlow.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "PortManager")]
    public class DocumentsController : ControllerBase
    {
        private readonly HarborService _harborService;

        public DocumentsController(HarborService harborService)
        {
            _harborService = harborService;
        }

        // PUT /api/documents/{id}/verify
        [HttpPut("{id}/verify")]
        public async Task<IActionResult> VerifyDocument(int id)
        {
            var document = await _harborService.VerifyDocumentAsync(id);
            if (document == null)
            {
                return NotFound("Document not found.");
            }
            return Ok(document);
        }
    }
}
