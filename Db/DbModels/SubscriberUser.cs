using System.ComponentModel.DataAnnotations.Schema;

namespace Db.DbModels;

[Table("SubscriberUsers", Schema = "suprusr")]
public class SubscriberUser
{
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    public Guid SubscriberId { get; set; }
    public Subscriber Subscriber { get; set; } = default!;
}