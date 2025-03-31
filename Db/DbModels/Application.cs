using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.DbModels;
public class Application
{
    public Guid ApplicationId { get; set; }
    public string MicrosoftGraphApiAppId { get; set; }

    public Subscriber Subscriber { get; set; }
    public List<Secret> Secrets { get; set; } = [];

}
