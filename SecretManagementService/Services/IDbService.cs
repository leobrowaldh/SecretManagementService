using SMSFunctionApp.Models;
using SMSFunctionApp.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretManagementService.Services;
public interface IDbService
{
    /// <summary>
    /// Sets the session context variables for the current connection in the used repositories.
    /// </summary>
    /// <param name="contextVariables"></param>
    /// <returns></returns>
    void SetContext(Dictionary<string, object?> contextVariables);
    void SetExecutingUser(string executingUser);
    Task<SecretNotificationInfo?> GetNotificationInfoAsync(Guid secretId, Guid appId);
    Task UpdateLastNotifiedAsync(Guid secretId, DateTime dateTime);
    Task<List<SecretDto>> GetAllSecretsAsync();
    Task AddNewSecretsAsync(List<SecretDto> secrets);
    Task DeleteSecretAsync(Guid secretId);
}
