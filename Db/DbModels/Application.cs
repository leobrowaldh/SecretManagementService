using System.ComponentModel.DataAnnotations.Schema;
using Db.Shared;

namespace Db.DbModels;

[Table("Applications", Schema = "adm")]
public class Application
{
    public Guid ApplicationId { get; set; }
    public string ExternalApplicationId { get; set; } = string.Empty;
    public EnIdentityProvider ExternalProvider { get; set; }
    public string StrExternalProvider
    {
        get => ExternalProvider.ToString();
        set { }
    }
    public string ClientId { get; set; } = string.Empty; //AppId in Azure EntraId
    public bool Seeded { get; set; }
    public bool ContactByEmail { get; set; }
    public bool ContactBySMS { get; set; }
    public bool ContactByApiEndpoint { get; set; }
    public List<PhoneApplication> PhoneApplications { get; set; } = [];
    public List<EmailApplication> EmailApplications { get; set; } = [];
    public Subscriber? Subscriber { get; set; }
    public Guid? SubscriberId { get; set; }
    public List<Secret> Secrets { get; set; } = [];
    public bool IsDeleted { get; set; }
}
