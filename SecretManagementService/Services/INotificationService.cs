using Microsoft.AspNetCore.DataProtection;
using SecretManagementService.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretManagementService.Services;
public interface INotificationService
{
    public Task<(SecretNotificationInfo? notificationDto, bool shouldNotify)> FetchNotificationInfoAsync(string secretId);
}
