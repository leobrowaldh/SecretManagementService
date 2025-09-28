using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace SecretManagementService.Services;
public class SmsService: ISmsService
{
    private readonly IConfiguration _configuration;

    public SmsService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendSmsAsync(string phoneNumber, string message)
    {
        var accountSid = _configuration["twilio-account-sid"];
        var authToken = _configuration["twilio-api-token"];
        var fromPhoneNumber = _configuration["twilio-phone-number"];
        TwilioClient.Init(accountSid, authToken);
        var messageResource = await MessageResource.CreateAsync(
            body: message,
            from: new PhoneNumber(fromPhoneNumber),
            to: new PhoneNumber(phoneNumber)
        );
        if (messageResource.ErrorCode != null)
        {
            throw new InvalidOperationException($"Failed to send SMS: {messageResource.ErrorMessage}");
        }
    }
}
    