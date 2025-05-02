using SecretManagementService.Models.Dtos;
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
    public Task<SecretNotificationInfo?> GetNotificationInfoAsync(string secretId);
}
