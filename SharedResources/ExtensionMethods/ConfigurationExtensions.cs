using Microsoft.Extensions.Configuration;
using Azure.Identity;

namespace SharedResources.ExtensionMethods;


public static class ConfigurationExtensions
{
    public static IConfigurationBuilder ConfigureKeyVault(this IConfigurationBuilder builder)
    {
        var basePath = Directory.GetCurrentDirectory();

        if (!File.Exists(Path.Combine(basePath, "designsettings.json")))
        {
            basePath = AppContext.BaseDirectory;
        }

        // Create a TEMPORARY builder
        var tempConfig = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("designsettings.json", optional: true)
            .AddJsonFile("local.settings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var keyVaultUri = tempConfig["KEY_VAULT_URI"]
            ?? throw new ArgumentNullException("KEY_VAULT_URI not found in configuration");

        var environment = tempConfig["ENVIRONMENT"]
            ?? throw new ArgumentNullException("ENVIRONMENT not found in configuration");

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

