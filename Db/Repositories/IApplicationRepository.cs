using Db.DbModels;

namespace Db.Repositories;

public interface IApplicationRepository : IGenericRepository<Application>
{
    Task<Application?> GetApplicationsByExternalIdAsync(string externalId);
}
