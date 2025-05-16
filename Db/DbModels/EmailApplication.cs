
using System.ComponentModel.DataAnnotations.Schema;

namespace Db.DbModels;

[Table("EmailApplications", Schema = "suprusr")]
public class EmailApplication
{
    public Guid EmailId { get; set; }
    public Email Email { get; set; } = default!;

    public Guid ApplicationId { get; set; }
    public Application Application { get; set; } = default!;
}