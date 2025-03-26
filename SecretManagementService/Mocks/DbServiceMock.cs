using SecretManagementService.Models.Dtos;
using SecretManagementService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretManagementService.Mocks;
public class MockNotificationService : IDbService
{
    private static readonly List<SecretNotificationDto> _notifications = new()
    {
        // SecretId: {Type}_{DaysToExpire}_{DaysSinceLastNotification}
        new SecretNotificationDto { SecretId = "Email_30d_Null", AppId = "App_Email_30d_Null", DisplayName = "Email - 30d - Null", OwnerId = null, EndDateTime = DateTime.UtcNow.AddDays(30), LastTimeNotified = null, Emails = new() { "email1@example.com" }, PhoneNumbers = new(), ApiEndpoints = new() },
        new SecretNotificationDto { SecretId = "Phone_30d_Null", AppId = "App_Phone_30d_Null", DisplayName = "Phone - 30d - Null", OwnerId = null, EndDateTime = DateTime.UtcNow.AddDays(30), LastTimeNotified = null, Emails = new(), PhoneNumbers = new() { "+46701234567" }, ApiEndpoints = new() },
        new SecretNotificationDto { SecretId = "API_30d_Null", AppId = "App_API_30d_Null", DisplayName = "API - 30d - Null", OwnerId = null, EndDateTime = DateTime.UtcNow.AddDays(30), LastTimeNotified = null, Emails = new(), PhoneNumbers = new(), ApiEndpoints = new() { "https://api1.com" } },
        new SecretNotificationDto { SecretId = "EmailAPI_30d_Null", AppId = "App_EmailAPI_30d_Null", DisplayName = "Email & API - 30d - Null", OwnerId = null, EndDateTime = DateTime.UtcNow.AddDays(30), LastTimeNotified = null, Emails = new() { "email2@example.com" }, PhoneNumbers = new(), ApiEndpoints = new() { "https://api2.com" } },
        new SecretNotificationDto { SecretId = "PhoneAPI_30d_Null", AppId = "App_PhoneAPI_30d_Null", DisplayName = "Phone & API - 30d - Null", OwnerId = null, EndDateTime = DateTime.UtcNow.AddDays(30), LastTimeNotified = null, Emails = new(), PhoneNumbers = new() { "+4745678901" }, ApiEndpoints = new() { "https://api3.com" } },

        new SecretNotificationDto { SecretId = "Email_15d_15dAgo", AppId = "App_Email_15d_15dAgo", DisplayName = "Email - 15d - 15dAgo", OwnerId = null, EndDateTime = DateTime.UtcNow.AddDays(15), LastTimeNotified = DateTime.UtcNow.AddDays(-15), Emails = new() { "email3@example.com" }, PhoneNumbers = new(), ApiEndpoints = new() },
        new SecretNotificationDto { SecretId = "Phone_15d_15dAgo", AppId = "App_Phone_15d_15dAgo", DisplayName = "Phone - 15d - 15dAgo", OwnerId = null, EndDateTime = DateTime.UtcNow.AddDays(15), LastTimeNotified = DateTime.UtcNow.AddDays(-15), Emails = new(), PhoneNumbers = new() { "+358501234567" }, ApiEndpoints = new() },
        new SecretNotificationDto { SecretId = "API_15d_15dAgo", AppId = "App_API_15d_15dAgo", DisplayName = "API - 15d - 15dAgo", OwnerId = null, EndDateTime = DateTime.UtcNow.AddDays(15), LastTimeNotified = DateTime.UtcNow.AddDays(-15), Emails = new(), PhoneNumbers = new(), ApiEndpoints = new() { "https://api4.com" } },
        new SecretNotificationDto { SecretId = "EmailAPI_15d_15dAgo", AppId = "App_EmailAPI_15d_15dAgo", DisplayName = "Email & API - 15d - 15dAgo", OwnerId = null, EndDateTime = DateTime.UtcNow.AddDays(15), LastTimeNotified = DateTime.UtcNow.AddDays(-15), Emails = new() { "email4@example.com" }, PhoneNumbers = new(), ApiEndpoints = new() { "https://api5.com" } },
        new SecretNotificationDto { SecretId = "PhoneAPI_15d_15dAgo", AppId = "App_PhoneAPI_15d_15dAgo", DisplayName = "Phone & API - 15d - 15dAgo", OwnerId = null, EndDateTime = DateTime.UtcNow.AddDays(15), LastTimeNotified = DateTime.UtcNow.AddDays(-15), Emails = new(), PhoneNumbers = new() { "+4798765432" }, ApiEndpoints = new() { "https://api6.com" } },

        new SecretNotificationDto { SecretId = "Email_15d_1mAgo", AppId = "App_Email_15d_1mAgo", DisplayName = "Email - 15d - 1mAgo", OwnerId = null, EndDateTime = DateTime.UtcNow.AddDays(15), LastTimeNotified = DateTime.UtcNow.AddMinutes(-1), Emails = new() { "email5@example.com" }, PhoneNumbers = new(), ApiEndpoints = new() },
        new SecretNotificationDto { SecretId = "Phone_15d_1mAgo", AppId = "App_Phone_15d_1mAgo", DisplayName = "Phone - 15d - 1mAgo", OwnerId = null, EndDateTime = DateTime.UtcNow.AddDays(15), LastTimeNotified = DateTime.UtcNow.AddMinutes(-1), Emails = new(), PhoneNumbers = new() { "+45712345678" }, ApiEndpoints = new() },
        new SecretNotificationDto { SecretId = "API_15d_1mAgo", AppId = "App_API_15d_1mAgo", DisplayName = "API - 15d - 1mAgo", OwnerId = null, EndDateTime = DateTime.UtcNow.AddDays(15), LastTimeNotified = DateTime.UtcNow.AddMinutes(-1), Emails = new(), PhoneNumbers = new(), ApiEndpoints = new() { "https://api7.com" } },

        new SecretNotificationDto { SecretId = "Email_5d_25dAgo", AppId = "App_Email_5d_25dAgo", DisplayName = "Email - 5d - 25dAgo", OwnerId = null, EndDateTime = DateTime.UtcNow.AddDays(5), LastTimeNotified = DateTime.UtcNow.AddDays(-25), Emails = new() { "email6@example.com" }, PhoneNumbers = new(), ApiEndpoints = new() },
        new SecretNotificationDto { SecretId = "Phone_5d_25dAgo", AppId = "App_Phone_5d_25dAgo", DisplayName = "Phone - 5d - 25dAgo", OwnerId = null, EndDateTime = DateTime.UtcNow.AddDays(5), LastTimeNotified = DateTime.UtcNow.AddDays(-25), Emails = new(), PhoneNumbers = new() { "+47234567890" }, ApiEndpoints = new() },
        new SecretNotificationDto { SecretId = "API_5d_25dAgo", AppId = "App_API_5d_25dAgo", DisplayName = "API - 5d - 25dAgo", OwnerId = null, EndDateTime = DateTime.UtcNow.AddDays(5), LastTimeNotified = DateTime.UtcNow.AddDays(-25), Emails = new(), PhoneNumbers = new(), ApiEndpoints = new() { "https://api8.com" } }
    };

    public Task<bool> ShouldNotifyAsync(string secretId, out SecretNotificationDto notificationDto)
    {
        notificationDto = _notifications.FirstOrDefault(n => n.SecretId == secretId) ?? new SecretNotificationDto { SecretId = secretId, AppId = "UnknownApp", DisplayName = "Unknown", OwnerId = null, EndDateTime = DateTime.UtcNow, LastTimeNotified = null, Emails = new(), PhoneNumbers = new(), ApiEndpoints = new() };
        return Task.FromResult(notificationDto.LastTimeNotified == null || notificationDto.LastTimeNotified < DateTime.UtcNow.AddDays(-10));
    }
}
