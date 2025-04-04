using Db.DbModels;
using DbRepos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Repositories;
public class PhoneRepository : GenericRepository<Phone>
{
    public PhoneRepository(SmsDbContext dbContext) : base(dbContext) { }

    protected override IQueryable<Phone> ApplyCustomFilter(IQueryable<Phone> query, bool seeded, string filter)
    {
        if (!string.IsNullOrWhiteSpace(filter))
        {
            return query.Where(s => s.PhoneNumber.ToLower().Contains(filter) && s.Seeded == seeded);
        }
        return query;
    }
}
