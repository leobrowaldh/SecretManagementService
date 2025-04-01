using Azure.Identity;
using Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SharedResources.ExtensionMethods;

namespace Db;

// This class is used to create the database context for the EF Core tools
// we need some configuration to be able to create the context
public class DesignTimeSmsDbContextFactory : IDesignTimeDbContextFactory<SmsDbContext>
{
    public SmsDbContext CreateDbContext(string[] args)
    {

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("local.settings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();


        //***DEBUG

        var configuration = builder.Build();

        Console.WriteLine("Configuration Variables:");
        foreach (var kvp in configuration.AsEnumerable())
        {
            Console.WriteLine($"{kvp.Key}: {kvp.Value}");
        }
        //***

        var environment = configuration["ENVIRONMENT"];
        Console.WriteLine($"Environment: {environment}");

        try
        {
            builder.ConfigureKeyVault();
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine($"Configuration variable missing:\n{ex.Message}");
        }

        configuration = builder.Build();

        var connectionString = configuration.GetConnectionString("SecretManagementServiceContext")
                               ?? Environment.GetEnvironmentVariable("SecretManagementServiceContext");

        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException($"Connection string is missing");

        var optionsBuilder = new DbContextOptionsBuilder<SmsDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new SmsDbContext(optionsBuilder.Options, configuration);
    }
}




