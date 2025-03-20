using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using SecretManagementService.Services;

var builder = FunctionsApplication.CreateBuilder(args);

ConfigureKeyVault(builder);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddHttpClient(name: "AzureAuth",
    configureClient: options =>
    {
        options.BaseAddress = new Uri("https://login.microsoftonline.com/");
        options.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
    });

builder.Build().Run();




static void ConfigureKeyVault(FunctionsApplicationBuilder builder)
{
    try
    {
        // Fetch the Key Vault URI from environment variables or local.settings.json
        var keyVaultUri = builder.Configuration["KEY_VAULT_URI"]
            ?? throw new ArgumentNullException("KEY_VAULT_URI not found in configuration");

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? throw new ArgumentNullException("ASPNETCORE_ENVIRONMENT not found in configuration");

        var excludeLocalDevelopment = (environment is not null && environment != "Local");
        var excludeManagedIdentity = !excludeLocalDevelopment;

        if (!string.IsNullOrEmpty(keyVaultUri))
        {
            // Add Azure Key Vault to the IConfiguration pipeline,
            // excluding unneccesary credential sources for performance.
            builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUri),
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
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while configuring Key Vault: {ex.Message}");
        throw;
    }
}
