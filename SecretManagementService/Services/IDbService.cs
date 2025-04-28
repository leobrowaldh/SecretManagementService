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
    /// Sets the session context variables for the current connection.
    /// </summary>
    /// <param name="contextVariables"></param>
    /// <returns></returns>
    public Task SetContextAsync(Dictionary<string, object?> contextVariables);
    public Task<SecretNotificationInfo?> GetNotificationInfoAsync(string secretId);
}
