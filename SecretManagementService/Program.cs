using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using SecretManagementService.Services;
using Microsoft.Azure.Functions.Worker;
using SecretManagementService.Mocks;
using SendGrid.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Db;
using SharedResources.ExtensionMethods;
using Db.Repositories;
using Db.DbModels;
using Microsoft.Data.SqlClient;
using Db.Factories;
using Db.Helpers;
using Azure.Identity;

var builder = FunctionsApplication.CreateBuilder(args);

//builder.Configuration.ConfigureKeyVault();
//not working when deployed

var keyVaultUri = builder.Configuration["KEY_VAULT_URI"];

if (string.IsNullOrEmpty(keyVaultUri))
{
    Console.WriteLine("KEY_VAULT_URI not found in configuration");
}
//TODO: optimize DefaultAzureCredential
else { builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential()); }

builder.ConfigureFunctionsWebApplication();

// Application Insights:
builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

// Services
builder.Services.AddScoped<IAzureTokenService, AzureTokenService>();
builder.Services.AddScoped<IGraphApiService, GraphApiService>();
builder.Services.AddScoped<ISecretsService, SecretsService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISmsService, SmsService>();
builder.Services.AddScoped<IDbService, DbServiceMock2>(); //***MOCKING!!***

//DbRepos
builder.Services.AddScoped<IGenericRepository<Secret>, SecretRepository>();
builder.Services.AddScoped<IGenericRepository<Application>, ApplicationRepository>();
builder.Services.AddScoped<IGenericRepository<Subscriber>, SubscriberRepository>();
builder.Services.AddScoped<IGenericRepository<ApiEndpoint>, ApiEndpointRepository>();
builder.Services.AddScoped<IEmailRepository, EmailRepository>();
builder.Services.AddScoped<IPhoneRepository, PhoneRepository>();


builder.Services.AddDbContext<SmsDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SecretManagementServiceContext"));
});

builder.Services.AddHttpContextAccessor();


// For Azure authentication with client secret
builder.Services.AddHttpClient(name: "AzureAuth",
    configureClient: options =>
    {
        options.BaseAddress = new Uri("https://login.microsoftonline.com/");
        options.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
    });

builder.Services.AddHttpClient(name: "GraphApi",
    configureClient: options =>
    {
        options.BaseAddress = new Uri("https://graph.microsoft.com/v1.0/");
        options.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
    });

// SendGrid: email service
builder.Services.AddSendGrid(options =>
{
    // Fetch the SendGrid API key from Key Vault
    var sendGridApiKey = builder.Configuration["sendgrid-api-key"];
    if (string.IsNullOrEmpty(sendGridApiKey))
    {
        Console.WriteLine("sendgrid-api-key is not found in Key Vault");
    }

    options.ApiKey = sendGridApiKey;
});


SqlAlwaysEncryptedLocalConfig.RegisterAzureKeyVaultProvider();

builder.Build().Run();


