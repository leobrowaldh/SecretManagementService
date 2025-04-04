using Db.DbModels;
using DbRepos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Repositories;
public class ApplicationRepository : GenericRepository<Application>
{
    public ApplicationRepository(SmsDbContext dbContext) : base(dbContext) { }
    protected override IQueryable<Application> ApplyCustomFilter(IQueryable<Application> query, bool seeded, string filter)
        => query.Where(a => a.Seeded == seeded);

}
