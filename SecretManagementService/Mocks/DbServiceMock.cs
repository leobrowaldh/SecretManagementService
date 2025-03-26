using SecretManagementService.Models;
using SecretManagementService.Models.Dtos;
using SecretManagementService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretManagementService.Mocks;
public class DbServiceMock : IDbService
{
    private readonly string _secretIdToTry = "Notify_All_30d_Null";
    private static readonly List<SecretNotificationInfo> _notifications = new()
    {
        new SecretNotificationInfo
        {
            DaysUntilSecretExpires = 30,
            SecretId = "Notify_All_30d_Null",
            AppId = "App_Notify_All_30d_Null",
            DisplayName = "Notify All - 30d - Null",
            OwnerId = null,
            EndDateTime = DateTime.UtcNow.AddDays(30),
            LastTimeNotified = null,
            ContactMethod = new ContactMethod
            {
                IsEmail = true,
                IsSMS = true,
                IsApiEndpoint = true,
                Emails = ["email1@example.com"],
                PhoneNumbers = ["+46701234567"],
                ApiInfo = new ApiInfo
                {
                    SecretId = "api-secret-1",
                    BaseUrl = "https://api1.com",
                    QueryParameters = new(),
                    Headers = new(),
                    Method = HttpMethod.Post,
                    BodyTemplate = null
                }
            }
        },
        new SecretNotificationInfo
        {
            DaysUntilSecretExpires = 30,
            SecretId = "Notify_All_15d_15dAgo",
            AppId = "App_Notify_All_15d_15dAgo",
            DisplayName = "Notify All - 15d - 15dAgo",
            OwnerId = null,
            EndDateTime = DateTime.UtcNow.AddDays(15),
            LastTimeNotified = DateTime.UtcNow.AddDays(-15),
            ContactMethod = new ContactMethod
            {
                IsEmail = true,
                IsSMS = true,
                IsApiEndpoint = true,
                Emails = ["email2@example.com"],
                PhoneNumbers = ["+4687654321"],
                ApiInfo = new ApiInfo
                {
                    SecretId = "api-secret-2",
                    BaseUrl = "https://api2.com",
                    QueryParameters = new(),
                    Headers = new(),
                    Method = HttpMethod.Post,
                    BodyTemplate = null
                }
            }
        },
        new SecretNotificationInfo
        {
            DaysUntilSecretExpires = 30,
            SecretId = "Notify_All_15d_1mAgo",
            AppId = "App_Notify_All_15d_1mAgo",
            DisplayName = "Notify All - 15d - 1mAgo",
            OwnerId = null,
            EndDateTime = DateTime.UtcNow.AddDays(15),
            LastTimeNotified = DateTime.UtcNow.AddMinutes(-1),
            ContactMethod = new ContactMethod
            {
                IsEmail = true,
                IsSMS = true,
                IsApiEndpoint = true,
                Emails = ["email3@example.com"],
                PhoneNumbers = ["+4698765432"],
                ApiInfo = new ApiInfo
                {
                    SecretId = "api-secret-3",
                    BaseUrl = "https://api3.com",
                    QueryParameters = new(),
                    Headers = new(),
                    Method = HttpMethod.Post,
                    BodyTemplate = null
                }
            }
        },
        new SecretNotificationInfo
        {
            DaysUntilSecretExpires = 30,
            SecretId = "Notify_All_5d_25dAgo",
            AppId = "App_Notify_All_5d_25dAgo",
            DisplayName = "Notify All - 5d - 25dAgo",
            OwnerId = null,
            EndDateTime = DateTime.UtcNow.AddDays(5),
            LastTimeNotified = DateTime.UtcNow.AddDays(-25),
            ContactMethod = new ContactMethod
            {
                IsEmail = true,
                IsSMS = true,
                IsApiEndpoint = true,
                Emails = ["email4@example.com"],
                PhoneNumbers = ["+46123456789"],
                ApiInfo = new ApiInfo
                {
                    SecretId = "api-secret-4",
                    BaseUrl = "https://api4.com",
                    QueryParameters = new(),
                    Headers = new(),
                    Method = HttpMethod.Post,
                    BodyTemplate = null
                }
            }
        }
    };

    public Task<SecretNotificationInfo?> GetNotificationInfoAsync(string secretId)
    {
        var notificationDto = _notifications.FirstOrDefault(n => n.SecretId == _secretIdToTry);
        return Task.FromResult(notificationDto);
    }
}

