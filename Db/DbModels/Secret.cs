using System.ComponentModel.DataAnnotations.Schema;

namespace Db.DbModels;

[Table("Secrets", Schema = "usr")]
public class Secret
{
    public Guid SecretId { get; set; }
    public string MicrosoftGraphApiKeyId { get; set; }
    public string? DisplayName { get; set; }
    public required DateTime EndDateTime { get; set; }
    public DateTime? LastTimeNotified { get; set; }
    public bool Seeded { get; set; }
    public required Application Application { get; set; }
    public Guid ApplicationId { get; set; }

}
