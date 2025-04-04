using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.DbModels;
public class Subscriber
{
    public Guid SubscriberId { get; set; }
    public String? MicrosoftGraphOwnerId { get; set; }
    public bool Seeded { get; set; }

    public List<Application> Applications { get; set; } = [];
    public List<Secret> Secrets { get; set; } = [];
}
