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
    public class SchedulesController : ControllerBase
    {
        private readonly HarborService _harborService;

        public SchedulesController(HarborService harborService)
        {
            _harborService = harborService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<Schedule>>> GetAllSchedules()
        {
            var schedules = await _harborService.GetSchedulesAsync();
            return Ok(schedules);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleRequest request)
        {
            var schedule = await _harborService.CreateScheduleAsync(request.ServiceRequestId, request.BerthNumber, request.Arrival, request.Departure);
            if (schedule == null)
            {
                return NotFound("Service Request not found or could not be approved.");
            }
            return Ok(schedule);
        }
    }
}
