using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SecretManagementService.Models;
using SecretManagementService.Models.Response;
using SecretManagementService.Services;
using System.Collections.Generic;

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
    [QueueOutput("expiringSecrets-queue")]  //  This binds the function output to the queue
    public async Task<IList<string>?> FetchSecretsAsync(
        [TimerTrigger("0 0 8 * * *", RunOnStartup = true)] TimerInfo myTimer)
    {
        _logger.LogInformation("Timer trigger function FetchExpiringSecrets executed at: {Current DateTime}", DateTime.Now);

        var expiringSecrets = await _secretsService.GetExpiringSecrets(_daysUntilSecretsExpire);

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

    [Function("ProcessExpiringSecrets")]
    public void ProcessSecretsAsync(
    [QueueTrigger("expiringSecrets-queue")] string message,
    FunctionContext context)
    {
        var logger = context.GetLogger("ProcessExpiringSecrets");
        var secret = JsonSerializer.Deserialize<ExpiringSecret>(message);

        logger.LogInformation("Processing secret: {secretId}", secret.KeyId);
    }
}
