using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSFunctionApp.Helpers;
public static class AzureAuthHelper
{
    public static async Task<IEnumerable<SecurityKey>> GetSigningKeysAsync(string tenantId)
    {
        var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            $"https://login.microsoftonline.com/{tenantId}/v2.0/.well-known/openid-configuration",
            new OpenIdConnectConfigurationRetriever());

        var config = await configManager.GetConfigurationAsync(CancellationToken.None);
        return config.SigningKeys;
    }

}
