using Db.DbModels;
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

    protected override IQueryable<Subscriber> ApplyCustomFilter(IQueryable<Subscriber> query, bool seeded, string filter)
        => query.Where(a => a.Seeded == seeded);
}
