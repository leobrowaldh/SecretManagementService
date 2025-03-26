using Microsoft.Extensions.Logging;
using SecretManagementService.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretManagementService.Services;
public class NotificationService : INotificationService
{
    private readonly IDbService _dbService;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(IDbService dbService, ILogger<NotificationService> logger)
    {
        _dbService = dbService;
        _logger = logger;
    }

    public async Task<SecretNotificationInfo> FetchNotificationInfoAsync(string secretId)
    {
        var secretNotificationInfo = await _dbService.GetNotificationInfoAsync(secretId);
        if (secretNotificationInfo == null)
        {
            //if the secret is registered in our app, we need to have the stored notification info.
            //is logging needed, or is it enough to throw?
            //_logger.LogError("No notification info found for secretId: {secretId}", secretId);
            throw new InvalidOperationException($"No notification info found for secretId {secretId}");
        }

        return secretNotificationInfo;
    }

    public async Task NotifyAsync(SecretNotificationInfo secretNotificationInfo)
    {
        if (secretNotificationInfo.ContactMethod.IsEmail)
        {
            //send email => send grid nuget package, or logic app
        }
        if (secretNotificationInfo.ContactMethod.IsSMS)
        {
            //send sms => twilio
        }
        if (secretNotificationInfo.ContactMethod.IsApiEndpoint)
        {
            //send api request
        }
    }
}
