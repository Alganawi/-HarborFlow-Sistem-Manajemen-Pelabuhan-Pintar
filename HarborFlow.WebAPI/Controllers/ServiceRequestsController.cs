using HarborFlow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HarborFlow.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly HarborService _harborService;

        public ServiceRequestsController(HarborService harborService)
        {
            _harborService = harborService;
        }

        // POST /api/servicerequests
        [HttpPost]
        public async Task<IActionResult> CreateServiceRequest([FromBody] CreateServiceRequestRequest request)
        {
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out var userId)){
                return Unauthorized();
            }

            var serviceRequest = await _harborService.CreateServiceRequestAsync(request.VesselId, userId, request.ServiceType);
            if (serviceRequest == null)
            {
                return NotFound("Vessel or User not found.");
            }
            return Ok(serviceRequest);
        }

        // POST /api/servicerequests/{id}/documents
        [HttpPost("{id}/documents")]
        public async Task<IActionResult> AddDocument(int id, [FromBody] AddDocumentRequest request)
        {
            var document = await _harborService.AddDocumentToServiceRequestAsync(id, request.DocumentName, request.FilePath);
            if (document == null)
            {
                return NotFound("Service Request not found.");
            }
            return Ok(document);
        }

        // GET /api/servicerequests/{id}/documents
        [HttpGet("{id}/documents")]
        public async Task<ActionResult<List<Document>>> GetDocuments(int id)
        {
            var documents = await _harborService.GetDocumentsForServiceRequestAsync(id);
            return Ok(documents);
        }

        // POST /api/servicerequests/{id}/cargo
        [HttpPost("{id}/cargo")]
        public async Task<IActionResult> AddCargo(int id, [FromBody] AddCargoRequest request)
        {
            var cargo = await _harborService.AddCargoToRequestAsync(id, request.Description, request.Weight, request.IsHazardous);
            if (cargo == null)
            {
                return NotFound("Service Request not found.");
            }
            return Ok(cargo);
        }

        // GET /api/servicerequests/{id}/cargo
        [HttpGet("{id}/cargo")]
        public async Task<ActionResult<List<Cargo>>> GetCargo(int id)
        {
            var cargos = await _harborService.GetCargoForRequestAsync(id);
            return Ok(cargos);
        }
    }
}
