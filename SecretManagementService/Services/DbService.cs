using Db;
using Db.DbModels;
using Db.Factories;
using Db.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SecretManagementService.Models;
using SMSFunctionApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretManagementService.Services;
public class DbService : IDbService
{
    private readonly IGenericRepository<Secret> _secretRepo;
    private readonly IPhoneRepository _phoneRepository;
    private readonly IEmailRepository _emailRepository;
    public DbService(IGenericRepository<Secret> secretRepo, IPhoneRepository phoneRepository, IEmailRepository emailRepository)
    {
        _secretRepo = secretRepo;
        _phoneRepository = phoneRepository;
        _emailRepository = emailRepository;
    }

    public void SetContext(Dictionary<string, object?> contextVariables)
    {
        _secretRepo.SetContext(contextVariables);
        _phoneRepository.SetContext(contextVariables);
        _emailRepository.SetContext(contextVariables);
    }
    public void SetExecutingUser(string executingUser)
    {
        _secretRepo.SetExecutingUser(executingUser);
        _phoneRepository.SetExecutingUser(executingUser);
        _emailRepository.SetExecutingUser(executingUser);
    }

    public async Task<SecretNotificationInfo?> GetNotificationInfoAsync(Guid secretId)
    {
        // Step 1: Fetch the Secret entity with its non-sensitive fields (e.g., Subscriber, Application)
        var secret = await _secretRepo.ReadItemAsync(secretId, false);

        if (secret == null)
        {
            return null;
        }

        // Step 2: Fetch the encrypted Phones, Emails separately using your ADO.NET repositories
        var phones = await _phoneRepository.GetPhonesBySecretIdAsync(secret.SecretId); // ADO.NET call for Phones (decrypting)
        var emails = await _emailRepository.GetEmailsBySecretIdAsync(secret.SecretId); // ADO.NET call for Emails (decrypting)

        // Step 3: Map everything into the SecretNotificationInfo object
        var notificationInfo = new SecretNotificationInfo
        {
            SecretId = secretId,
            AppId = secret.Application?.ApplicationId ?? Guid.Empty,
            DisplayName = secret.DisplayName,
            EndDateTime = secret.EndDateTime,
            LastTimeNotified = secret.LastTimeNotified,
            ContactMethod = new ContactMethod
            {
                IsEmail = secret.ContactByEmail,
                IsSMS = secret.ContactBySMS,
                IsApiEndpoint = secret.ContactByApiEndpoint,
                Emails = emails.Select(e => e.EmailAddress).ToList(),
                PhoneNumbers = phones.Select(p => p.PhoneNumber).ToList(),
                ApiInfo = new ApiInfo
                {
                    //Not yet implemented, add logic here when models are fully defined.
                    SecretId = secretId,
                    BaseUrl = secret.ApiEndpoints.FirstOrDefault()?.BaseUrl ?? string.Empty
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
