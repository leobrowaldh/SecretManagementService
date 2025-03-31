using Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

// This class is used to create the database context for the EF Core tools
// we need some configuration to be able to create the context
public class DesignTimeSmsDbContextFactory : IDesignTimeDbContextFactory<SmsDbContext>
{
    public SmsDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("local.settings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("SecretManagementServiceContext")
                               ?? Environment.GetEnvironmentVariable("SecretManagementServiceContext");

        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Connection string is missing!");

        var optionsBuilder = new DbContextOptionsBuilder<SmsDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new SmsDbContext(optionsBuilder.Options, configuration);
    }
}

