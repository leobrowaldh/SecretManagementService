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

    //currently for azure, could be others.
    public async Task SyncDatabaseSecretsWithSource()
    {
        _logger.LogInformation("Fetching secrets from GraphApi...");
        var appData = await _graphApiService.GetAppDataAsync();

        _logger.LogInformation("Fetching secrets from database...");
        //if using more external providers, the appllications ExternalProvider should be checked first, to fetch the correct secrets.
        //Currently we only use azure secrets and applications, so there is no need.
        var dbSecrets = await _dbService.GetAllSecretsAsync();

        List<SecretDto> fetchedSecrets = appData.ToSecretDtoList();

        // hashsets increase performance for large dataset comparisons
        var fetchedKeyIds = fetchedSecrets
            .Select(fs => fs.ExternalSecretId)
            .ToHashSet();

        var dbSecretIds = dbSecrets
            .Select(ds => ds.ExternalSecretId)
            .ToHashSet();

        var newSecretIds = fetchedKeyIds.Except(dbSecretIds).ToList();      // in fetched, not in db
        _logger.LogInformation("New secrets found: {Count}", newSecretIds.Count);
        var deletedSecretIds = dbSecretIds.Except(fetchedKeyIds).ToList();  // in db, not in fetched
        _logger.LogInformation("Deleted secrets found: {Count}", deletedSecretIds.Count);

        if (newSecretIds.Count > 0)
        {
            _logger.LogInformation("Adding new secrets to database...");

            var newSecretDtos = fetchedSecrets
                .Where(fs => newSecretIds.Contains(fs.ExternalSecretId))
                .ToList();

            await _dbService.AddNewSecretsAsync(newSecretDtos);
        }
        if (deletedSecretIds.Count > 0)
        {
            _logger.LogInformation("Deleting secrets from database...");
            foreach (var externalSecretId in deletedSecretIds)
            {
                if (externalSecretId == null)
                {
                    _logger.LogWarning("ExternalSecretId is null, skipping deletion.");
                    continue;
                }
                SecretDto secret = dbSecrets.First(ds => ds.ExternalSecretId == externalSecretId);
                await _dbService.DeleteSecretAsync((Guid)secret.SecretId!);
            }
        }
    }

    public async Task<List<SecretDto>?> GetExpiringSecretsAsync(int _daysUntilSecretsExpire)
    {

        return await _dbService.GetExpiringSecretsAsync(_daysUntilSecretsExpire);
    }
}
