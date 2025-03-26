using SecretManagementService.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretManagementService.Services;
public class DbService : IDbService
{
    public Task<bool> ShouldNotifyAsync(string secretId, out SecretNotificationDto notificationDto)
    {
        throw new NotImplementedException();
    }
}
