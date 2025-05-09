using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretManagementService.Models;

public record FetchedSecret
{
    public required string AppObjectId { get; set; }
    public required string AppId { get; set; }
    public string? DisplayName { get; set; }
    public required DateTime EndDateTime { get; set; }
    public required string KeyId { get; set; }
    public string?[]? OwnerId { get; set; }

}
