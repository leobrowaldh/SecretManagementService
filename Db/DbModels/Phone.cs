using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.DbModels;

[Table("Phones", Schema = "suprusr")]
public class Phone
{
    public Guid PhoneId { get; set; }
    [Required]
    [MaxLength(30)]
    [Phone]
    public required string PhoneNumber { get; set; }
    public bool Seeded { get; set; }
    public List<PhoneApplication> PhoneApplications { get; set; } = [];
    public Subscriber Subscriber { get; set; } = default!;
    public required Guid SubscriberId { get; set; }
    public bool IsDeleted { get; set; }
}
