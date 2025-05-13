using Db.DbModels;
using Db.Repositories;
using SecretManagementService.Models;
using SMSFunctionApp.Models;
using System.Data;

namespace SecretManagementService.Services;
public class DbService : IDbService
{
    private readonly IGenericRepository<Application> _applicationRepo;
    public DbService(IGenericRepository<Application> applicationRepo)
    {
        _applicationRepo = applicationRepo;
    }

    public void SetContext(Dictionary<string, object?> contextVariables)
    {
        _applicationRepo.SetContext(contextVariables);
    }
    public void SetExecutingUser(string executingUser)
    {
        _applicationRepo.SetExecutingUser(executingUser);
    }

    public async Task<SecretNotificationInfo?> GetNotificationInfoAsync(Guid secretId, Guid appId)
    {
        var application = await _applicationRepo.ReadItemAsync(appId, false);

        if (application == null)
        {
            return null;
        }
        var secret = application.Secrets.FirstOrDefault(s => s.SecretId == secretId);
        if (secret == null)
        {
            return null;
        }

        var notificationInfo = new SecretNotificationInfo
        {
            SecretId = secretId,
            AppId = appId,
            DisplayName = secret.DisplayName,
            EndDateTime = secret.EndDateTime,
            LastTimeNotified = secret.LastTimeNotified,
            ContactMethod = new ContactMethod
            {
                IsEmail = application.ContactByEmail,
                IsSMS = application.ContactBySMS,
                IsApiEndpoint = application.ContactByApiEndpoint,
                Emails = application.Emails.Select(e => e.EmailAddress).ToList(),
                PhoneNumbers = application.Phones.Select(p => p.PhoneNumber).ToList(),
                ApiInfo = new ApiInfo
                {
                    //Not yet implemented, add logic here when models are fully defined.
                    SecretId = secretId,
                    BaseUrl = application.Subscriber.ApiEndpoint?.BaseUrl ?? "no url found",
                }
            }
        };

        // Step 4: Calculate days until expiration
        notificationInfo.DaysUntilSecretExpires = (secret.EndDateTime - DateTime.UtcNow).Days;

        return notificationInfo;
    }

    public Task UpdateLastNotifiedAsync(Guid secretId, DateTime dateTime)
    {
        throw new NotImplementedException();
    }
}
