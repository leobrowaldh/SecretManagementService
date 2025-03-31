using Microsoft.Extensions.Configuration;
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
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ISmsService _smsService;

    public NotificationService(IDbService dbService, ILogger<NotificationService> logger, IEmailService emailService, IConfiguration configuration, ISmsService smsService)
    {
        _dbService = dbService;
        _logger = logger;
        _emailService = emailService;
        _configuration = configuration;
        _smsService = smsService;
    }

    public async Task<SecretNotificationInfo> FetchNotificationInfoAsync(string secretId)
    {
        var secretNotificationInfo = await _dbService.GetNotificationInfoAsync(secretId);
        if (secretNotificationInfo == null)
        {
            //if the secret is registered in our app, we need to have the stored notification info.
            throw new InvalidOperationException($"No notification info found for secretId {secretId}");
        }

        return secretNotificationInfo;
    }

    public async Task NotifyAsync(SecretNotificationInfo secretNotificationInfo)
    {
        if (secretNotificationInfo.ContactMethod.IsEmail)
        {
            foreach (var email in secretNotificationInfo.ContactMethod.Emails)
            {
                _logger.LogInformation("Sending email notification for secret {SecretId}", secretNotificationInfo.SecretId);
                await _emailService.SendEmailAsync(email, "Secret Expiration Notification",
                    $"<p>Your secret <strong>{secretNotificationInfo.DisplayName}</strong> is expiring on {secretNotificationInfo.EndDateTime}.</p>" +
                    $"<p>Access your account: <a href='{_configuration["SMS_URL"]}'>{_configuration["SMS_URL"]}</a></p>");
            }
        }
        if (secretNotificationInfo.ContactMethod.IsSMS)
        {
            _logger.LogInformation("SMS Notification Service is currently Disabled, upgraded Twilio account is required.");
            //Cannot verify swedish numbers by sms, and cannot verify by voicecall in free acounts.
            //So we need to upgrade account to test this.

            //foreach (var phoneNumber in secretNotificationInfo.ContactMethod.PhoneNumbers)
            //{
            //    _logger.LogInformation("Sending SMS notification for secret {SecretId}", secretNotificationInfo.SecretId);
            //    await _smsService.SendSmsAsync(phoneNumber, $"Your secret {secretNotificationInfo.DisplayName} is expiring on {secretNotificationInfo.EndDateTime}." +
            //        $"<p>Access your account: <a href='{_configuration["SMS_URL"]}'>{_configuration["SMS_URL"]}</a></p>");
            //}
        }
        if (secretNotificationInfo.ContactMethod.IsApiEndpoint)
        {
            //send api request
        }
    }
}
