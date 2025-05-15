using Db.DbModels;
using Db.Helpers;
using DbRepos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Repositories;
public class ApplicationRepository : GenericRepository<Application>, IApplicationRepository
{
    public ApplicationRepository(SmsDbContext dbContext) : base(dbContext) { }
    protected override IQueryable<Application> ApplyCustomFilter(IQueryable<Application> query, bool seeded, string filter)
        => query.Where(a => a.Seeded == seeded);

    public async Task<Application?> GetApplicationsByExternalIdAsync(string externalId)
    {
        return await SqlQueryInjector.RunWithUserAsync(_connection, _sessionContext, _executingUser, async () =>
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.ExternalApplicationId == externalId);
        });
    }



}
