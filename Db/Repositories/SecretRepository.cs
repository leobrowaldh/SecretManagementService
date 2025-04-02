using Db.DbModels;
using DbRepos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Repositories;
public class SecretRepository : GenericRepository<Secret>
{
    public SecretRepository(SmsDbContext dbContext) : base(dbContext) { }

    protected override IQueryable<Secret> ApplyCustomFilter(IQueryable<Secret> query, bool seeded, string filter)
    {
        if (!string.IsNullOrWhiteSpace(filter))
        {
            return query.Where(s => s.DisplayName.ToLower().Contains(filter));
        }
        return query;
    }
}
