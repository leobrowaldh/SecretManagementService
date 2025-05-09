using SecretManagementService.Models;

namespace SecretManagementService.Services;
public interface ISecretsService
{
    public Task<List<FetchedSecret>?> GetExpiringSecretsAsync(int _daysUntilSecretsExpire);
    public Task SyncDatabaseSecretsWithSource();
}
