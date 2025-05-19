using System.ComponentModel.DataAnnotations.Schema;

namespace Db.DbModels;

[Table("Users", Schema = "suprusr")]
public class User
{
    public Guid UserId { get; set; }
    public List<SubscriberUser> SubscriberUsers { get; set; } = [];
    public bool IsDeleted { get; set; }
}
