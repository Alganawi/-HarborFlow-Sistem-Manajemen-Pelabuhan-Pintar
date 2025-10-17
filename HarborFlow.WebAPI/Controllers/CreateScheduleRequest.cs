using System;

namespace HarborFlow.WebAPI.Controllers
{
    public class CreateScheduleRequest
    {
        public int ServiceRequestId { get; set; }
        public int BerthNumber { get; set; }
        public DateTime Arrival { get; set; }
        public DateTime Departure { get; set; }
    }
}
