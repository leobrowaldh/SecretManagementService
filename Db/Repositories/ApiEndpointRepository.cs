using Db.DbModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Repositories;
public class ApiEndpointRepository : GenericRepository<ApiEndpoint>
{
    public ApiEndpointRepository(SmsDbContext dbContext) : base(dbContext) { }

    protected override IQueryable<ApiEndpoint> ApplyCustomFilter(IQueryable<ApiEndpoint> query, bool seeded, string filter)
    {
        if (!string.IsNullOrWhiteSpace(filter))
        {
            return query.Where(s => s.BaseUrl.ToLower().Contains(filter) && s.Seeded == seeded);
        }
        return query;
    }
}
