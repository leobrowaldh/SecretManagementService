using System.ComponentModel.DataAnnotations.Schema;
using Db.Shared;

namespace Db.DbModels;

[Table("Secrets", Schema = "usr")]
public class Secret
{
    public Guid SecretId { get; set; }
    public string ExternalSecretId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public required DateTime EndDateTime { get; set; }
    public DateTime? LastTimeNotified { get; set; }
    public bool Seeded { get; set; }
    public Application Application { get; set; } = default!;
    public required Guid ApplicationId { get; set; }
    public bool IsDeleted { get; set; }

}
