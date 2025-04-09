using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.DbModels;

[Table("Emails", Schema = "suprusr")]
public class Email
{
    public Guid EmailId { get; set; }
    [EmailAddress]
    public string EmailAddress { get; set; }
    public bool Seeded { get; set; }
    public List<Secret> Secrets { get; set; } = [];
}
