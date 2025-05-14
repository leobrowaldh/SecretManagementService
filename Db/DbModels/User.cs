using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.DbModels;

[Table("Users", Schema = "suprusr")]
public class User
{
    public Guid UserId { get; set; }
    public List<Subscriber> Subscribers { get; set; } = [];
    public bool IsDeleted { get; set; }
}
