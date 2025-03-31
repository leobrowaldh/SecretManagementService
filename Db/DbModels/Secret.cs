using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.DbModels;
public class Secret
{
    public Guid SecretId { get; set; }
    public string MicrosoftGraphApiKeyId { get; set; }
    public string? DisplayName { get; set; }
    public required DateTime EndDateTime { get; set; }
    public DateTime? LastTimeNotified { get; set; }
    public bool ContactByEmail { get; set; }
    public bool ContactBySMS { get; set; }
    public bool ContactByApiEndpoint { get; set; }

    public Application? Application { get; set; }
    public Subscriber Subscriber { get; set; }
    public List<Phone> Phones { get; set; } = [];
    public List<Email> Emails { get; set; } = [];
    public List<ApiEndpoint> ApiEndpoints { get; set; } = [];
}
