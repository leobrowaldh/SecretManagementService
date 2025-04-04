using SecretManagementService.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretManagementService.Services;
public class DbService : IDbService
{
    //context.Database.ExecuteSqlRaw("EXEC sp_set_session_context @key='TenantId', @value={0}", tenantId);
    //this sets the session context for the current connection, for Row based security in the database
    //wait, fetch the corresponding subscriberid with the tenantid, and send in the subscriberid in the context.
    //This way we avoid tight coupling to azure.
    //EXEC sp_set_session_context @key = 'SubscriberId', @value = '<SubscriberId>';

    public Task<SecretNotificationInfo?> GetNotificationInfoAsync(string secretId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ShouldNotifyAsync(string secretId, out SecretNotificationInfo notificationDto)
    {
        throw new NotImplementedException();
    }
}
