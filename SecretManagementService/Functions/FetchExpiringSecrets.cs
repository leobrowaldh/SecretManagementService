using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SecretManagementService.Services;
using Microsoft.Extensions.Configuration;
using SMSFunctionApp.Models.DTOs;

namespace SecretManagementService.Functions;

public class FetchExpiringSecrets
{
    private readonly ILogger _logger;
    private readonly ISecretsService _secretsService;
    private readonly int _daysUntilSecretsExpire;

    public FetchExpiringSecrets(ILogger<FetchExpiringSecrets> logger, ISecretsService secretsService, IConfiguration configuration)
    {
        _logger = logger;
        _secretsService = secretsService;
        _daysUntilSecretsExpire = configuration.GetValue<int>("DAYS_UNTIL_SECRET_EXPIRES");
    }

    [Function(nameof(FetchExpiringSecrets))]
    [QueueOutput("expiringsecrets-queue")]  //  This binds the function output to the queue
    public async Task<IList<string>?> FetchSecretsAsync(
        [TimerTrigger("0 0 8 * * *", RunOnStartup = true)] TimerInfo myTimer)
    {
        _logger.LogInformation("Timer trigger function FetchExpiringSecrets executed at: {Current DateTime}", DateTime.Now);

        List<SecretDto>? expiringSecrets = await _secretsService.GetExpiringSecretsAsync(_daysUntilSecretsExpire);

        if (expiringSecrets == null || expiringSecrets.Count == 0)
        {
            _logger.LogInformation("No expiring secrets found.");
            return null;  // No messages will be sent to the queue
        }

        _logger.LogInformation("Pushing {n secrets} expiring secrets to the queue.", expiringSecrets.Count);

        // Serialize and return secrets as queue messages
        var messages = new List<string>();
        foreach (var secret in expiringSecrets)
        {
            messages.Add(JsonSerializer.Serialize(secret));
        }

        return messages; //  This automatically sends messages to the queue
    }
}
