using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretManagementService.Models.Dtos;
public class SecretNotificationInfo
{
    public required string SecretId { get; set; }
    public required string AppId { get; set; }
    public string? DisplayName { get; set; }
    public string?[]? OwnerId { get; set; }
    public required DateTime EndDateTime { get; set; }
    public DateTime? LastTimeNotified { get; set; }
    public required ContactMethod ContactMethod { get; set; }
    public bool ShouldNotify =>
        LastTimeNotified == null ||
        LastTimeNotified < DateTime.UtcNow.AddDays(-15) ||
        (EndDateTime < DateTime.UtcNow.AddDays(5) && LastTimeNotified < DateTime.UtcNow.AddDays(-5)) ||
        EndDateTime.Day == DateTime.UtcNow.Day;
}