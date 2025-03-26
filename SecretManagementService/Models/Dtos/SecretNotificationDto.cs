using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretManagementService.Models.Dtos;
public class SecretNotificationDto
{
    public required string SecretId { get; set; }
    public required string AppId { get; set; }
    public string? DisplayName { get; set; }
    public string?[]? OwnerId { get; set; }
    public required DateTime EndDateTime { get; set; }
    public DateTime? LastTimeNotified { get; set; }
    public List<string> Emails { get; set; } = [];
    public List<string> PhoneNumbers { get; set; } = [];
    public List<string> ApiEndpoints { get; set; } = [];
}
