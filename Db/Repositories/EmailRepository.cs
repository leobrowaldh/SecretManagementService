using Db.DbModels;
using DbRepos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Repositories;
public class EmailRepository : GenericRepository<Email>
{
    public EmailRepository(SmsDbContext dbContext) : base(dbContext) { }

    protected override IQueryable<Email> ApplyCustomFilter(IQueryable<Email> query, bool seeded, string filter)
    {
        if (!string.IsNullOrWhiteSpace(filter))
        {
            return query.Where(s => s.EmailAddress.ToLower().Contains(filter));
        }
        return query;
    }
}
