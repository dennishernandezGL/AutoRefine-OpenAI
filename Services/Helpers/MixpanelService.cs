using Microsoft.AspNetCore.Mvc;
using Services.Logging;
using Services.Services;

namespace Services.Helpers;

public class MixpanelService
{
    private readonly IConfiguration _configuration;
    
    public MixpanelService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<IEnumerable<dynamic>> RetrieveLogsByContext([FromBody] MonitoringControllerService.RetrieveLogsByContextRequest request)
    {
        if (request.DateRange == null || request.DateRange.StartDate == default || request.DateRange.EndDate == default)
        {
            return [];
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
            return [];
        }

        return logs;
    }

    public async Task<IEnumerable<dynamic>>  RetrieveLogs([FromBody] MonitoringControllerService.DateRange dateRange)
    {
        var loggingFacade = LoggingFacade.GetInstance(LoggingConnections.MixPanel, _configuration);
        var logs = await loggingFacade.RetrieveLogsAsync(dateRange.StartDate, dateRange.EndDate, dateRange.EventName);

        if (logs == null || !logs.Any())
        {
            return [];
        }

        return logs;
    }
}