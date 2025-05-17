using Microsoft.AspNetCore.Mvc;
using Services.Logging;

namespace Services.Services
{
    [ApiController]
    [Route("api/[controller]")]
    public class MonitoringControllerService : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public MonitoringControllerService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("retrieve-logs")]
        public async Task<IActionResult> RetrieveLogs([FromBody] DateRange dateRange)
        {
            if (dateRange == null || dateRange.StartDate == default || dateRange.EndDate == default)
            {
                return BadRequest("Invalid date range provided.");
            }

            var loggingFacade = LoggingFacade.GetInstance(LoggingConnections.MixPanel, _configuration);
            var logs = await loggingFacade.RetrieveMixPanelLogsAsync(dateRange.StartDate, dateRange.EndDate, dateRange.EventName);

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

            var loggingFacade = LoggingFacade.GetInstance(LoggingConnections.MixPanel, _configuration);
            var logs = await loggingFacade.RetrieveLogsByContextAsync(
                log => request.Context.ComponentName == log.ComponentName &&
                       request.Context.Environment == log.Environment &&
                       request.Context.InstanceIdentifier == log.InstanceIdentifier &&
                       request.Context.LoggerUser == log.LoggerUser,
                request.DateRange.StartDate,
                request.DateRange.EndDate,
                request.EventName);

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