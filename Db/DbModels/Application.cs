using System.ComponentModel.DataAnnotations.Schema;

namespace Db.DbModels;

[Table("Applications", Schema = "adm")]
public class Application
{
    public Guid ApplicationId { get; set; }
    public Guid MicrosoftGraphApiAppId { get; set; }
    public bool Seeded { get; set; }
    public bool ContactByEmail { get; set; }
    public bool ContactBySMS { get; set; }
    public bool ContactByApiEndpoint { get; set; }
    public List<Phone> Phones { get; set; } = [];
    public List<Email> Emails { get; set; } = [];
    public required Subscriber Subscriber { get; set; }
    public Guid SubscriberId { get; set; }
    public List<Secret> Secrets { get; set; } = [];

}
