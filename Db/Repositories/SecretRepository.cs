using Db.DbModels;
using Db.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Db.Repositories;
public class SecretRepository : GenericRepository<Secret>, ISecretRepository
{
    public SecretRepository(SmsDbContext dbContext) : base(dbContext) { }

    public Task<List<Secret>> GetExpiringSecrets(int daysUntilExpiration)
    {
        return SqlQueryInjector.RunWithUserAsync(_connection, _sessionContext, _executingUser, async () =>
        {
            var query = _dbSet.AsNoTracking()
                .Where(s => s.EndDateTime < DateTime.UtcNow.AddDays(daysUntilExpiration))
                .OrderBy(s => s.EndDateTime);
            return await query.ToListAsync();
        });
    }

    protected override IQueryable<Secret> ApplyCustomFilter(IQueryable<Secret> query, bool seeded, string filter)
    {
        if (!string.IsNullOrWhiteSpace(filter))
        {
            return query.Where(s => s.DisplayName.ToLower().Contains(filter) && s.Seeded == seeded);
        }
        return query;
    }

}
