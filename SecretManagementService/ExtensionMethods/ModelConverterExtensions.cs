using Db.DbModels;
using SecretManagementService.Models.Response;
using SMSFunctionApp.Models.DTOs;

namespace SMSFunctionApp.ExtensionMethods;
public static class ModelConverterExtensions
{
    public static SecretDto ToSecretDto(this Secret secret)
    {
        return new SecretDto
        {
            SecretId = secret.SecretId,
            ApplicationId = secret.ApplicationId,
            DisplayName = secret.DisplayName,
            EndDateTime = secret.EndDateTime,
            LastTimeNotified = secret.LastTimeNotified,
            Seeded = secret.Seeded
        };
    }

    public static List<SecretDto> ToSecretDtoListOfExpiringSecrets(this GraphApiGenericResponse<GraphApiApplicationResponse> appData, int daysUntilSecretsExpire)
    {
        List<SecretDto> expiringSecrets = appData.value
            .SelectMany(app => app.passwordCredentials
                .Where(cred => cred.endDateTime < DateTime.Now.AddDays(daysUntilSecretsExpire))
                .Select(cred => new SecretDto
                {
                    SecretId = Guid.TryParse(cred.keyId, out var parsedKeyId) ? parsedKeyId : null,
                    ApplicationId = Guid.TryParse(app.id, out var parsedApplicationId) ? parsedApplicationId : null,
                    AppId = Guid.TryParse(app.appId, out var parsedAppId) ? parsedAppId : null,
                    DisplayName = cred.displayName,
                    EndDateTime = cred.endDateTime

                })
            ).ToList();
        return expiringSecrets;
    }

    public static List<SecretDto> ToSecretDtoList(this GraphApiGenericResponse<GraphApiApplicationResponse> appData)
    {
        List<SecretDto> expiringSecrets = appData.value
            .SelectMany(app => app.passwordCredentials
                .Select(cred => new SecretDto
                {
                    SecretId = Guid.TryParse(cred.keyId, out var parsedKeyId) ? parsedKeyId : null,
                    ApplicationId = Guid.TryParse(app.id, out var parsedApplicationId) ? parsedApplicationId : null,
                    AppId = Guid.TryParse(app.appId, out var parsedAppId) ? parsedAppId : null,
                    DisplayName = cred.displayName,
                    EndDateTime = cred.endDateTime

                })
            ).ToList();
        return expiringSecrets;
    }
}
