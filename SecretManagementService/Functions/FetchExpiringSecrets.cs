//using System;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Text.Json;
//using Microsoft.Azure.Functions.Worker;
//using Microsoft.Extensions.Logging;
//using SecretManagementService.Models;
//using SecretManagementService.Models.Response;
//using SecretManagementService.Services;

//namespace SecretManagementService.Functions;

//public class FetchExpiringSecrets
//{
//    private readonly ILogger _logger;
//    private readonly IGraphApiService _graphApiService;
//    private readonly int _daysUntilSecretsExpire = 30; 

//    public FetchExpiringSecrets(ILogger<FetchExpiringSecrets> logger, IGraphApiService graphApiService)
//    {
//        _logger = logger;
//        _graphApiService = graphApiService;
//    }

//    [Function("FetchExpiringSecrets")]
//    //running every day at at 8:00:00 AM
//    public async Task RunAsync([TimerTrigger("0 0 8 * * *", RunOnStartup = true)] TimerInfo myTimer)
//    {
//        _logger.LogInformation($"Timer trigger function FetchExpiringSecrets executed at: {DateTime.Now}");

//        try
//        {
//            _logger.LogInformation("Checking for expiring secrets...");
//            var expiringSecrets = await _graphApiService.GetExpiringSecrets(_daysUntilSecretsExpire);
//            if (expiringSecrets == null || expiringSecrets.Count == 0)
//            {
//                _logger.LogInformation("No expiring secrets found.");
//                return;
//            }
//            _logger.LogInformation($"Successfully fetched {expiringSecrets.Count} expiring secrets.");
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "FetchExpiringSecrets function failed.");
//            throw; // Ensure Azure logs this failure as an unhandled exception
//        }

//        if (myTimer.ScheduleStatus is not null)
//        {
//            _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
//        }
//    }
//}
 