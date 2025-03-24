using Microsoft.Extensions.Logging;
using SecretManagementService.Models;
using SecretManagementService.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretManagementService.Services;
public class SecretsService : ISecretsService
{
    private readonly IGraphApiService _graphApiService;
    private readonly ILogger<SecretsService> _logger;

    public SecretsService(IGraphApiService graphApiService, ILogger<SecretsService> logger)
    {
        _graphApiService = graphApiService;
        _logger = logger;
    }

    public async Task<List<ExpiringSecret>?> GetExpiringSecrets(int _daysUntilSecretsExpire)
    {
        var appData = await _graphApiService.GetAppDataAsync();

        List<ExpiringSecret>? expiringSecrets = appData?.value
            .SelectMany(app => app.passwordCredentials
                .Where(cred => cred.endDateTime < DateTime.Now.AddDays(_daysUntilSecretsExpire))
                .Select(cred => new ExpiringSecret
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
