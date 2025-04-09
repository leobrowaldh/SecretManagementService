using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.DbModels;

[Table("Applications", Schema = "suprusr")]
public class Application
{
    public Guid ApplicationId { get; set; }
    public string MicrosoftGraphApiAppId { get; set; }
    public bool Seeded { get; set; }

    public Subscriber Subscriber { get; set; }
    public List<Secret> Secrets { get; set; } = [];

}
