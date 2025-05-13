using Microsoft.AspNetCore.DataProtection;
using SMSFunctionApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretManagementService.Services;
public interface INotificationService
{
    public Task<SecretNotificationInfo> FetchNotificationInfoAsync(Guid secretId, Guid appId);
    public Task NotifyAsync(SecretNotificationInfo secretNotificationInfo);
}
