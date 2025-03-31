using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.DbModels;
public class Email
{
    public Guid EmailId { get; set; }
    [EmailAddress]
    public string EmailAddress { get; set; }
    public List<Secret> Secrets { get; set; } = [];
}
