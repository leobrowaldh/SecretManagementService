using Microsoft.Extensions.Configuration;
using Azure.Identity;

namespace SharedResources.ExtensionMethods;


public static class ConfigurationExtensions
{
    public static IConfigurationBuilder ConfigureKeyVault(this IConfigurationBuilder builder)
    {
        var keyVaultUri = Environment.GetEnvironmentVariable("KEY_VAULT_URI")
        ?? throw new ArgumentNullException("KEY_VAULT_URI not found in environment variables");

        var environment = Environment.GetEnvironmentVariable("ENVIRONMENT")
            ?? throw new ArgumentNullException("ENVIRONMENT not found in environment variables");

        var excludeLocalDevelopment = environment != "Local";
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

        return builder;
    }
}

