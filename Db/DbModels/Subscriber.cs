using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.DbModels;

[Table("Subscribers", Schema = "suprusr")]
public class Subscriber
{
    public Guid SubscriberId { get; set; }
    public String? SubscriberIdentifier { get; set; }
    public bool Seeded { get; set; }

    public List<Application> Applications { get; set; } = [];
    public List<Secret> Secrets { get; set; } = [];
    public List<User> Users { get; set; } = [];
}
