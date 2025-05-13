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
using SMSFunctionApp.Models;

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
        [QueueTrigger("expiringsecrets-queue")] CancellationToken cancellationToken, string message, FunctionContext context)
    {
        //another way of creating a logger, using functionContext, which provide function execution context.
        var logger = context.GetLogger("ProcessExpiringSecrets");

        var secret = JsonSerializer.Deserialize<FetchedSecret>(message);
        if (secret == null)
        {
            //logger.LogError("Message was null or empty: {message}", message);
            throw new InvalidOperationException($"Failed to deserialize message, message is null or empty: {message}");
        }

        logger.LogInformation("Calling notification service...");

        if (!Guid.TryParse(secret.KeyId, out var secretId))
        {
            _logger.LogWarning("Invalid GUID: {KeyId}", secret.KeyId);
            //Maybe forward to dead letter queue, or similar aproach, so it doesnt die silently.
            return; 
        }
        if (!Guid.TryParse(secret.AppId, out var applicationId))
        {
            _logger.LogWarning("Invalid GUID: {AppId}", secret.AppId);
            //Maybe forward to dead letter queue, or similar aproach, so it doesnt die silently.
            return;
        }
        SecretNotificationInfo secretNotificationInfo = await _notificationService.FetchNotificationInfoAsync(secretId, applicationId);

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
