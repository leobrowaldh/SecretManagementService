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
            return query.Where(s => s.DisplayName.ToLower().Contains(filter) && s.Seeded == seeded);
        }
        return query;
    }

    //we do not want to fetch encrypted data, so we override the ApplyIncludes method in order to exclude encrypted tables
    protected override IQueryable<Secret> ApplyIncludes(IQueryable<Secret> query, bool flat)
    {
        if (flat)
            return query;

        var navigations = _dbContext.Model.FindEntityType(typeof(Secret))?
                             .GetNavigations()
                             .Select(n => n.Name);

        if (navigations != null)
        {
            foreach (var navigation in navigations)
            {
                // Exclude the encrypted properties (Emails, Phones) from being included
                if (navigation != "Emails" && navigation != "Phones")
                {
                    query = query.Include(navigation);
                }
            }
        }

        return query;
    }

}
