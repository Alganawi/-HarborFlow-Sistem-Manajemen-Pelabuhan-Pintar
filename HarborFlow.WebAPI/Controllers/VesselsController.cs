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
    public class VesselsController : ControllerBase
    {
        private readonly HarborService _harborService;

        public VesselsController(HarborService harborService)
        {
            _harborService = harborService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<Vessel>>> GetAllVessels()
        {
            var vessels = await _harborService.GetAllVesselsAsync();
            return Ok(vessels);
        }

        [HttpPost]
        public async Task<ActionResult<Vessel>> CreateVessel([FromBody] CreateVesselRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            var newVessel = await _harborService.AddVesselAsync(request.Name, request.ImoNumber, request.Type, request.Capacity);

            return CreatedAtAction(nameof(GetAllVessels), new { id = newVessel.VesselID }, newVessel);
        }
    }
}
