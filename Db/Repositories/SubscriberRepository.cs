using Db.DbModels;
using DbRepos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Repositories;
public class SubscriberRepository : GenericRepository<Subscriber>
{
    public SubscriberRepository(SmsDbContext dbContext) : base(dbContext) { }

}
