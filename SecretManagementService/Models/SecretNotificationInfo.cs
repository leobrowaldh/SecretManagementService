using Microsoft.Extensions.Configuration;
using SecretManagementService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSFunctionApp.Models;
public class SecretNotificationInfo
{
    public int DaysUntilSecretExpires { get; set; }
    public required Guid SecretId { get; set; }
    public required Guid AppId { get; set; }
    public string? DisplayName { get; set; }
    public string?[]? OwnerId { get; set; }
    public required DateTime EndDateTime { get; set; }
    public DateTime? LastTimeNotified { get; set; }
    public required ContactMethod ContactMethod { get; set; }

    //Notification should be sent if:
    //1. The secret has never been notified
    //2. The secret has not been notified in the last half of the time until it expires
    //3. The secret expires in less than 5 days and has not been notified in the last 5 days
    //4. The secret expires today
    public bool ShouldNotify =>
        LastTimeNotified == null ||
        LastTimeNotified < DateTime.UtcNow.AddDays(-(DaysUntilSecretExpires / 2)) ||
        EndDateTime < DateTime.UtcNow.AddDays(5) && LastTimeNotified < DateTime.UtcNow.AddDays(-5) ||
        EndDateTime.Day == DateTime.UtcNow.Day;

}