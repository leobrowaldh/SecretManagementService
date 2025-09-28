using SMSFunctionApp.Models.DTOs;

namespace SecretManagementService.Services;
public interface ISecretsService
{
    public Task<List<SecretDto>?> GetExpiringSecretsAsync(int _daysUntilSecretsExpire);
    /// <summary>
    /// updates database with secrets from the source if changes are detected.
    /// </summary>
    /// <returns></returns>
    public Task SyncDatabaseSecretsWithSource();
}
