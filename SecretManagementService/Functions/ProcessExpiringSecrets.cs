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
using SecretManagementService.Models.Dtos;

namespace SecretManagementService.Functions;

public class ProcessExpiringSecrets
{
    private readonly ILogger _logger;
    private readonly INotificationService _notificationService;

    public ProcessExpiringSecrets(ILogger<ProcessExpiringSecrets> logger, INotificationService notificationService)
    {
        _logger = logger;
        _notificationService = notificationService;
    }

    [Function(nameof(ProcessExpiringSecrets))]
    public async Task ProcessSecretsAsync(
        [QueueTrigger("expiringSecrets-queue")] string message, FunctionContext context)
    {
        //another way of creating a logger, using functionContext, which provide function execution context.
        var logger = context.GetLogger("ProcessExpiringSecrets");

        var secret = JsonSerializer.Deserialize<ExpiringSecret>(message);
        if (secret == null)
        {
            //logger.LogError("Message was null or empty: {message}", message);
            throw new InvalidOperationException($"Failed to deserialize message, message is null or empty: {message}");
        }

        logger.LogInformation("Calling notification service...");
        SecretNotificationInfo secretNotificationInfo = await _notificationService.FetchNotificationInfoAsync(secret.KeyId);

        if (secretNotificationInfo.ShouldNotify)
        {
            await _notificationService.NotifyAsync(secretNotificationInfo);
        }
        else
        {
            logger.LogInformation("No notification needed at this point in time.");
        }    
    }
}
