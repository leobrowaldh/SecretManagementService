using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SMSFunctionApp.Models;
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

        _dbService.SetExecutingUser("AppScopedUser_BackgroundTasks");
    }

    public async Task<SecretNotificationInfo?> FetchNotificationInfoAsync(Guid secretId, Guid appId)
    {
        var secretNotificationInfo = await _dbService.GetNotificationInfoAsync(secretId, appId);
        if (secretNotificationInfo == null)
        {
            return null;
        }

        return secretNotificationInfo;
    }

    public async Task NotifyAsync(SecretNotificationInfo secretNotificationInfo)
    {
        if (secretNotificationInfo.ContactMethod.IsEmail)
        {
            if (secretNotificationInfo.ContactMethod.Emails == null || secretNotificationInfo.ContactMethod.Emails.Count == 0)
            {
                _logger.LogWarning("No email address found for application {ApplicationId}", secretNotificationInfo.Secret.ApplicationId);
                return;
            }
            foreach (var email in secretNotificationInfo.ContactMethod.Emails)
            {
                _logger.LogInformation("Sending email notification for secret {SecretId}", secretNotificationInfo.Secret.SecretId);
                await _emailService.SendEmailAsync(email, "Secret Expiration Notification",
                    $"<p>Your secret <strong>{secretNotificationInfo.Secret.DisplayName}</strong> is expiring on {secretNotificationInfo.Secret.EndDateTime}.</p>" +
                    $"<p>Access your account: <a href='{_configuration["SMS_URL"]}'>{_configuration["SMS_URL"]}</a></p>");
            }
            if (secretNotificationInfo.Secret.SecretId is not null)
            {
                await _dbService.UpdateLastNotifiedAsync((Guid)secretNotificationInfo.Secret.SecretId, DateTime.UtcNow);
            }
        }
        if (secretNotificationInfo.ContactMethod.IsSMS)
        {
            _logger.LogInformation("SMS Notification Service is currently Disabled, upgraded Twilio account is required.");
            //Cannot verify swedish numbers by sms, and cannot verify by voicecall in free acounts.
            //So we need to upgrade account to test this.
            //if (secretNotificationInfo.ContactMethod.PhoneNumbers == null || secretNotificationInfo.ContactMethod.PhoneNumbers.Count == 0)
            //{
            //    _logger.LogWarning("No phone number found for application {ApplicationId}", secretNotificationInfo.Secret.ApplicationId);
            //    return;
            //}
            //foreach (var phoneNumber in secretNotificationInfo.ContactMethod.PhoneNumbers)
            //{
            //    _logger.LogInformation("Sending SMS notification for secret {SecretId}", secretNotificationInfo.SecretId);
            //    await _smsService.SendSmsAsync(phoneNumber, $"Your secret {secretNotificationInfo.DisplayName} is expiring on {secretNotificationInfo.EndDateTime}." +
            //        $"<p>Access your account: <a href='{_configuration["SMS_URL"]}'>{_configuration["SMS_URL"]}</a></p>");
            //}
            //await _dbService.UpdateLastNotifiedAsync(secretNotificationInfo.SecretId, DateTime.UtcNow);
        }
        if (secretNotificationInfo.ContactMethod.IsApiEndpoint)
        {
            //send api request
        }
        else { _logger.LogWarning("No contact method found for secret {SecretId}", secretNotificationInfo.Secret.SecretId); }
    }
}
