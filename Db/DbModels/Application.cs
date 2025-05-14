using System.ComponentModel.DataAnnotations.Schema;

namespace Db.DbModels;

[Table("Applications", Schema = "adm")]
public class Application
{
    //ApplicationId = objectId in Azures entraId, can be used with other cloud provider applicationId too
    public Guid ApplicationId { get; set; }
    //AppId = AppId in Azures EntraId
    public Guid AppId { get; set; }
    public bool Seeded { get; set; }
    public bool ContactByEmail { get; set; }
    public bool ContactBySMS { get; set; }
    public bool ContactByApiEndpoint { get; set; }
    public List<Phone> Phones { get; set; } = [];
    public List<Email> Emails { get; set; } = [];
    public Subscriber? Subscriber { get; set; }
    public Guid? SubscriberId { get; set; }
    public List<Secret> Secrets { get; set; } = [];
    public bool IsDeleted { get; set; }
}
