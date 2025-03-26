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

public class ProcessExpiringSecrets
{
    private readonly ILogger _logger;
    private readonly INotificationService _notificationService;

    public ProcessExpiringSecrets(ILogger<FetchExpiringSecrets> logger, INotificationService notificationService)
    {
        _logger = logger;
        _notificationService = notificationService;
    }

    [Function(nameof(ProcessExpiringSecrets))]
    public void ProcessSecretsAsync(
        [QueueTrigger("expiringSecrets-queue")] string message, FunctionContext context)
    {
        //another way of creating a logger, using functionContext, which provide function execution context.
        var logger = context.GetLogger("ProcessExpiringSecrets");
        var secret = JsonSerializer.Deserialize<ExpiringSecret>(message);

        logger.LogInformation("Processing secret: {secretId}", secret.KeyId);
    }
}
