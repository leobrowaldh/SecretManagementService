using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.DbModels;
public class Phone
{
    public Guid PhoneId { get; set; }
    [Phone]
    public string PhoneNumber { get; set; }
    public bool Seeded { get; set; }
    public List<Secret> Secrets { get; set; } = [];
}
