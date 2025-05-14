using Db.DbModels;
using Db.Repositories;
using SecretManagementService.Models;
using SMSFunctionApp.ExtensionMethods;
using SMSFunctionApp.Models;
using SMSFunctionApp.Models.DTOs;
using System.Data;

namespace SecretManagementService.Services;
public class DbService : IDbService
{
    private readonly IGenericRepository<Application> _applicationRepo;
    private readonly IGenericRepository<Secret> _secretRepo;
    public DbService(IGenericRepository<Application> applicationRepo, IGenericRepository<Secret> secretRepo)
    {
        _applicationRepo = applicationRepo;
        _secretRepo = secretRepo;
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
            Secret = secret.ToSecretDto(),
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
                    BaseUrl = application.Subscriber?.ApiEndpoint?.BaseUrl ?? "no subscriber or no url found",
                }
            }
        };

        notificationInfo.DaysUntilSecretExpires = (secret.EndDateTime - DateTime.UtcNow).Days;

        return notificationInfo;
    }

    public async Task UpdateLastNotifiedAsync(Guid secretId, DateTime dateTime)
    {
        var secret = await _secretRepo.ReadItemAsync(secretId, true);
        if (secret != null)
        {
            secret.LastTimeNotified = dateTime;
            await _secretRepo.UpdateItemAsync(secret);
        }
        else
        {
            throw new InvalidOperationException($"Secret with id {secretId} not found.");
        }
    }

    public async Task<List<SecretDto>> GetAllSecretsAsync()
    {
        var secrets = await _secretRepo.ReadItemsAsync(true, "") 
            ?? throw new InvalidOperationException("No secrets found.");
        var dbSecrets = new List<SecretDto>();
        foreach (var secret in secrets)
        {
            dbSecrets.Add(secret.ToSecretDto());
        }
        return dbSecrets;
    }

    public Task AddNewSecretsAsync(List<SecretDto> secrets)
    {
        throw new NotImplementedException();
    }

    public Task DeleteSecretAsync(Guid secretId)
    {
        throw new NotImplementedException();
    }
}
