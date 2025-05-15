using Db.DbModels;
using Db.Shared;
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
                    ExternalSecretId = cred.keyId,
                    ExternalApplicationId = app.id,
                    ClientId = app.appId,
                    DisplayName = cred.displayName,
                    EndDateTime = cred.endDateTime,
                    ExternalProvider = EnIdentityProvider.Azure,
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
                    ExternalSecretId = cred.keyId,
                    ExternalApplicationId = app.id,
                    ClientId = app.appId,
                    DisplayName = cred.displayName,
                    EndDateTime = cred.endDateTime,
                    ExternalProvider = EnIdentityProvider.Azure,
                })
            ).ToList();
        return expiringSecrets;
    }

    public static Secret ToSecret(this SecretDto secretDto)
    {
        return new Secret
        {
            SecretId = secretDto.SecretId ?? Guid.Empty,
            ExternalSecretId = secretDto.ExternalSecretId ?? "",
            ApplicationId = secretDto.ApplicationId ?? Guid.Empty,
            DisplayName = secretDto.DisplayName,
            EndDateTime = secretDto.EndDateTime,
            LastTimeNotified = secretDto.LastTimeNotified,
            Seeded = secretDto.Seeded
        };
    }

    public static Application ToApplication(this SecretDto secretDto)
    {
        return new Application
        {
            ApplicationId = secretDto.ApplicationId ?? Guid.Empty,
            ExternalApplicationId = secretDto.ExternalApplicationId ?? "",
            ExternalProvider = secretDto.ExternalProvider,
            ClientId = secretDto.ClientId ?? "",
            Seeded = secretDto.Seeded
        };
    }
}
