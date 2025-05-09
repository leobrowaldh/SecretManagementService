using Microsoft.Extensions.Logging;
using SecretManagementService.Models;

namespace SecretManagementService.Services;
public class SecretsService : ISecretsService
{
    private readonly IGraphApiService _graphApiService;
    private readonly ILogger<SecretsService> _logger;
    private readonly IDbService _dbService;

    public SecretsService(IGraphApiService graphApiService, IDbService dbService, ILogger<SecretsService> logger)
    {
        _graphApiService = graphApiService;
        _dbService = dbService;
        _logger = logger;
    }

    public async Task SyncDatabaseSecretsWithSource()
    {
        var appData = await _graphApiService.GetAppDataAsync();
        var dbSecrets = _dbService.GetAllSecretsAsync();

        List<FetchedSecret>? expiringSecrets = appData?.value
            .SelectMany(app => app.passwordCredentials
                .Select(cred => new FetchedSecret
                {
                    AppObjectId = app.id,
                    AppId = app.appId,
                    DisplayName = cred.displayName,
                    EndDateTime = cred.endDateTime,
                    KeyId = cred.keyId

                })
            ).ToList();

        if (expiringSecrets == null || expiringSecrets.Count == 0)
        {
            _logger.LogInformation("No expiring secrets found.");
        }
        else
        {
            _logger.LogInformation("Successfully fetched {Count} expiring secrets.", expiringSecrets.Count);
        }

        return expiringSecrets;
    }

    public async Task<List<FetchedSecret>?> GetExpiringSecretsAsync(int _daysUntilSecretsExpire)
    {
        var appData = await _graphApiService.GetAppDataAsync();

        List<FetchedSecret>? expiringSecrets = appData?.value
            .SelectMany(app => app.passwordCredentials
                .Where(cred => cred.endDateTime < DateTime.Now.AddDays(_daysUntilSecretsExpire))
                .Select(cred => new FetchedSecret
                {
                    AppObjectId = app.id,
                    AppId = app.appId,
                    DisplayName = cred.displayName,
                    EndDateTime = cred.endDateTime,
                    KeyId = cred.keyId

                })
            ).ToList();

        if (expiringSecrets == null || expiringSecrets.Count == 0)
        {
            _logger.LogInformation("No expiring secrets found.");
        }
        else
        {
            _logger.LogInformation("Successfully fetched {Count} expiring secrets.", expiringSecrets.Count);
        }

        return expiringSecrets;
    }
}
