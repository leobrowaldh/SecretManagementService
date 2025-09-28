using System.ComponentModel.DataAnnotations.Schema;

namespace Db.DbModels;

[Table("PhoneApplications", Schema = "suprusr")]
public class PhoneApplication
{
    public Guid PhoneId { get; set; }
    public Phone Phone { get; set; } = default!;

    public Guid ApplicationId { get; set; }
    public Application Application { get; set; } = default!;
}