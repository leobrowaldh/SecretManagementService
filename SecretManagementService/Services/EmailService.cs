using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretManagementService.Services;
public class EmailService : IEmailService
{
    private readonly ISendGridClient _sendGridClient;
    private readonly ILogger<EmailService> _logger;

    public EmailService(ISendGridClient sendGridClient, ILogger<EmailService> logger)
    {
        _sendGridClient = sendGridClient;
        _logger = logger;
    }
    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var from = new EmailAddress("leo.browaldh@innofactor.com", "Secret Management Service");
        var to = new EmailAddress(toEmail);
        var msg = new SendGridMessage()
        {
            From = from,
            Subject = subject
        };
        msg.AddContent(MimeType.Html, body);
        msg.AddTo(to);

        var response = await _sendGridClient.SendEmailAsync(msg).ConfigureAwait(false);

        if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
        {
            _logger.LogError("Failed to send email to {toEmail}. Status code: {response.StatusCode}", toEmail, response.StatusCode);
        }
        else
        {
            _logger.LogInformation("Email sent to {toEmail}", toEmail);
        }
    }
}
