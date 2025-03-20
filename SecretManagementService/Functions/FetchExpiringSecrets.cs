using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace SecretManagementService.Functions;

public class FetchExpiringSecrets
{
    private readonly ILogger _logger;

    public FetchExpiringSecrets(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<FetchExpiringSecrets>();
    }

    [Function("FetchExpiringSecrets")]
    //running every day at at 8:00:00 AM
    public void Run([TimerTrigger("0 0 8 * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        
        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
        }
    }
}
