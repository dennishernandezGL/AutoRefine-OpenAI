using Microsoft.AspNetCore.Mvc;
using Services.Helpers;
using Services.Logging;

namespace Services.Services
{
    [ApiController]
    [Route("api/[controller]")]
    public class MonitoringControllerService : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly MixpanelService _mixpanelService;

        public MonitoringControllerService(IConfiguration configuration, MixpanelService mixpanelService)
        {
            _configuration = configuration;
            _mixpanelService = mixpanelService;
        }

        [HttpPost("retrieve-logs")]
        public async Task<IActionResult> RetrieveLogs([FromBody] DateRange dateRange)
        {
            if (dateRange == null || dateRange.StartDate == default || dateRange.EndDate == default)
            {
                return BadRequest("Invalid date range provided.");
            }

            var logs = await _mixpanelService.RetrieveLogs(dateRange);
            
            if (logs == null || !logs.Any())
            {
                return NotFound("No logs found for the specified date range.");
            }

            return Ok(logs);
        }

        [HttpPost("retrieve-logs-by-context")]
        public async Task<IActionResult> RetrieveLogsByContext([FromBody] RetrieveLogsByContextRequest request)
        {
            if (request.DateRange == null || request.DateRange.StartDate == default || request.DateRange.EndDate == default)
            {
                return BadRequest("Invalid date range provided.");
            }

            var logs = await _mixpanelService.RetrieveLogsByContext(request);
            
            if (logs == null || !logs.Any())
            {
                return NotFound("No logs found for the specified date range.");
            }

            return Ok(logs);
        }

        public class DateRange
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string EventName { get; set; }
        }

        public class RetrieveLogsByContextRequest
        {
            public DateRange DateRange { get; set; }
            public Context Context { get; set; }
            public string EventName { get; set; }
            
        }
    }
}