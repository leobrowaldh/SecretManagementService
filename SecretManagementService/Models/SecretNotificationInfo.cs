using SMSFunctionApp.Models.DTOs;

namespace SMSFunctionApp.Models;
public class SecretNotificationInfo
{
    public int DaysUntilSecretExpires { get; set; }
    public SecretDto Secret { get; set; } = new SecretDto();
    public required ContactMethod ContactMethod { get; set; }

    //Notification should be sent if:
    //1. The secret has never been notified
    //2. The secret has not been notified in the last half of the time until it expires
    //3. The secret expires in less than 5 days and has not been notified in the last 5 days
    //4. The secret expires today
    public bool ShouldNotify =>
        Secret.LastTimeNotified == null ||
        Secret.LastTimeNotified < DateTime.UtcNow.AddDays(-(DaysUntilSecretExpires / 2)) ||
        Secret.EndDateTime < DateTime.UtcNow.AddDays(5) && Secret.LastTimeNotified < DateTime.UtcNow.AddDays(-5) ||
        Secret.EndDateTime.Day == DateTime.UtcNow.Day;

}