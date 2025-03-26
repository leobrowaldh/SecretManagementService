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

    public async Task<(SecretNotificationInfo? notificationDto, bool shouldNotify)> FetchNotificationInfoAsync(string secretId)
    {
        var notificationDto = await _dbService.GetNotificationInfoAsync(secretId);
        if (notificationDto == null)
        {
            //if the secret is registered in our app, we need to have the stored notification info.
            _logger.LogError("No notification info found for secretId: {secretId}", secretId);
            throw new InvalidOperationException($"No notification info found for secretId {secretId}");
        }
        //When to notify:
        bool shouldNotify = notificationDto.LastTimeNotified == null || 
            notificationDto.LastTimeNotified < DateTime.UtcNow.AddDays(-15) ||
            (notificationDto.EndDateTime < DateTime.UtcNow.AddDays(5)
                && notificationDto.LastTimeNotified < DateTime.UtcNow.AddDays(-5)) ||
            notificationDto.EndDateTime.Day == DateTime.UtcNow.Day;

        return (notificationDto, shouldNotify);
    }
}
