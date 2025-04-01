using Microsoft.Extensions.Configuration;
using Azure.Identity;

namespace SharedResources.ExtensionMethods;


public static class ConfigurationExtensions
{
    public static void ConfigureKeyVault(this IConfigurationBuilder builder)
    {
        var configuration = builder.Build();

        var keyVaultUri = configuration["KEY_VAULT_URI"]
            ?? throw new ArgumentNullException("KEY_VAULT_URI not found in configuration");

        var environment = configuration["ENVIRONMENT"]
            ?? throw new ArgumentNullException("ENVIRONMENT not found in configuration");

        var excludeLocalDevelopment = (environment is not null && environment != "Local");
        var excludeManagedIdentity = !excludeLocalDevelopment;

        if (!string.IsNullOrEmpty(keyVaultUri))
        {
            // Add Azure Key Vault to the IConfiguration pipeline,
            // excluding unneccesary credential sources for performance.
            builder.AddAzureKeyVault(new Uri(keyVaultUri),
                new DefaultAzureCredential(
                    new DefaultAzureCredentialOptions()
                    {
                        ExcludeAzureCliCredential = true,
                        ExcludeAzureDeveloperCliCredential = true,
                        ExcludeAzurePowerShellCredential = true,
                        ExcludeEnvironmentCredential = true,
                        ExcludeInteractiveBrowserCredential = true,
                        ExcludeManagedIdentityCredential = excludeManagedIdentity,
                        ExcludeSharedTokenCacheCredential = true,
                        ExcludeVisualStudioCodeCredential = excludeLocalDevelopment,
                        ExcludeVisualStudioCredential = excludeLocalDevelopment,
                        ExcludeWorkloadIdentityCredential = true,
                    }));
        }
    }
}

