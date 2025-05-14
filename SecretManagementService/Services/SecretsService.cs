using Microsoft.Extensions.Logging;
using SMSFunctionApp.ExtensionMethods;
using SMSFunctionApp.Models.DTOs;

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
        _logger.LogInformation("Fetching secrets from GraphApi...");
        var appData = await _graphApiService.GetAppDataAsync();

        _logger.LogInformation("Fetching secrets from database...");
        var dbSecrets = await _dbService.GetAllSecretsAsync();

        List<SecretDto> fetchedSecrets = appData.ToSecretDtoList();

        // hashsets increase performance for large dataset comparisons
        var fetchedKeyIds = fetchedSecrets
            .Select(fs => fs.SecretId)
            .ToHashSet();

        var dbSecretIds = dbSecrets
            .Select(ds => ds.SecretId)
            .ToHashSet();

        var newSecretIds = fetchedKeyIds.Except(dbSecretIds).ToList();      // in fetched, not in db
        _logger.LogInformation("New secrets found: {Count}", newSecretIds.Count);
        var deletedSecretIds = dbSecretIds.Except(fetchedKeyIds).ToList();  // in db, not in fetched
        _logger.LogInformation("Deleted secrets found: {Count}", deletedSecretIds.Count);

        if (newSecretIds.Count > 0)
        {
            _logger.LogInformation("Adding new secrets to database...");

            var newSecretDtos = fetchedSecrets
                .Where(fs => newSecretIds.Contains(fs.SecretId))
                .ToList();

            await _dbService.AddNewSecretsAsync(newSecretDtos);
        }
        if (deletedSecretIds.Count > 0)
        {
            _logger.LogInformation("Deleting secrets from database...");
            foreach (var secretId in deletedSecretIds)
            {
                if (secretId == null)
                {
                    _logger.LogWarning("SecretId is null, skipping deletion.");
                    continue;
                }
                await _dbService.DeleteSecretAsync((Guid)secretId);
                _logger.LogInformation("Deleted secret with id {SecretId}", secretId);
            }
        }
    }

    //TODO: implement logic to get secrets from database, not graphapi
    public async Task<List<SecretDto>?> GetExpiringSecretsAsync(int _daysUntilSecretsExpire)
    {
        var appData = await _graphApiService.GetAppDataAsync();

        var expiringSecrets = appData.ToSecretDtoListOfExpiringSecrets(_daysUntilSecretsExpire);

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
