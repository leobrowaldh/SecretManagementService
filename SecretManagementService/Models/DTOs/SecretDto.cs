
namespace SMSFunctionApp.Models.DTOs;
public class SecretDto
{
    public Guid? SecretId { get; set; }
    public Guid? ApplicationId { get; set; }
    public Guid? AppId { get; set; }
    public string? DisplayName { get; set; }
    public DateTime EndDateTime { get; set; }
    public DateTime? LastTimeNotified { get; set; }
    public bool Seeded { get; set; }
}
