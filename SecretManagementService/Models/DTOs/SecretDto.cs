
using Db.Shared;

namespace SMSFunctionApp.Models.DTOs;
public class SecretDto
{
    public Guid? SecretId { get; set; }
    public string? ExternalSecretId { get; set; }
    public Guid? ApplicationId { get; set; }
    public string? ExternalApplicationId { get; set; }
    public EnIdentityProvider ExternalProvider { get; set; }
    public string? ClientId { get; set; } //AppId in Azure EntraId
    public string? DisplayName { get; set; }
    public DateTime EndDateTime { get; set; }
    public DateTime? LastTimeNotified { get; set; }
    public bool Seeded { get; set; }
}
