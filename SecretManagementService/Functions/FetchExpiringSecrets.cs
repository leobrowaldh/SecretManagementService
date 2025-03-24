using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SecretManagementService.Models;
using SecretManagementService.Models.Response;
using SecretManagementService.Services;

namespace SecretManagementService.Functions;

public class FetchExpiringSecrets
{
    private readonly ILogger _logger;
    private readonly ISecretsService _secretsService;
    private readonly int _daysUntilSecretsExpire = 30;

    public FetchExpiringSecrets(ILogger<FetchExpiringSecrets> logger, ISecretsService secretsService)
    {
        _logger = logger;
        _secretsService = secretsService;
    }

    [Function("FetchExpiringSecrets")]
    //running every day at at 8:00:00 AM
    public async Task RunAsync([TimerTrigger("0 0 8 * * *", RunOnStartup = true)] TimerInfo myTimer)
    {
        _logger.LogInformation($"Timer trigger function FetchExpiringSecrets executed at: {DateTime.Now}");

        var expiringSecrets = await _secretsService.GetExpiringSecrets(_daysUntilSecretsExpire);

        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
        }
    }
}
