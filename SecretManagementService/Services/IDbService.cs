using SecretManagementService.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretManagementService.Services;
public interface IDbService
{
    public Task<bool> ShouldNotifyAsync(string secretId, out SecretNotificationDto notificationDto);
}
