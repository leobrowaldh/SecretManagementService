using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Microsoft.Extensions.Configuration;

var builder = FunctionsApplication.CreateBuilder(args);

// Fetch the Key Vault URI from environment variables or local.settings.json
var keyVaultUri = builder.Configuration["KEY_VAULT_URI"];

if (!string.IsNullOrEmpty(keyVaultUri))
{
    // Add Azure Key Vault to the IConfiguration pipeline
    builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential());
}

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddHttpClient();

builder.Build().Run();
