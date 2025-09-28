using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SharedResources.ExtensionMethods;

namespace Db.Factories;

// This class is used to create the database context for the EF Core tools
// we need some configuration to be able to create the context
public class DesignTimeSmsDbContextFactory : IDesignTimeDbContextFactory<SmsDbContext>
{
    public SmsDbContext CreateDbContext(string[] args)
    {

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("designsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        builder.ConfigureKeyVault();

        var configuration = builder.Build();

        var connectionString = configuration.GetConnectionString("SecretManagementServiceContext")
                               ?? Environment.GetEnvironmentVariable("SecretManagementServiceContext");

        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException($"Connection string is missing");

        var optionsBuilder = new DbContextOptionsBuilder<SmsDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new SmsDbContext(optionsBuilder.Options, configuration);
    }
}




