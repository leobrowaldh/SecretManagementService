using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Db.DbModels;

[Table("Emails", Schema = "suprusr")]
public class Email
{
    public Guid EmailId { get; set; }
    [Required]
    [MaxLength(320)]
    [EmailAddress]
    public required string EmailAddress { get; set; }
    public bool Seeded { get; set; }
    public List<Application> Applications { get; set; } = [];
    public Subscriber Subscriber { get; set; }
    public Guid SubscriberId { get; set; }
}
