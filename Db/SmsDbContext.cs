using Db.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace Db;

public class SmsDbContext: DbContext
{
    private readonly IConfiguration _configuration;

    public SmsDbContext(DbContextOptions<SmsDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    //this env variables have to be set manually in the migrations runtime
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var keyVaultUri = _configuration["KEY_VAULT_URI"];
            if (string.IsNullOrEmpty(keyVaultUri))
            {
                throw new ArgumentNullException("KEY_VAULT_URI not found in configuration");
            }

            var environment = _configuration["ASPNETCORE_ENVIRONMENT"];
            if (string.IsNullOrEmpty(keyVaultUri))
            {
                throw new ArgumentNullException("ASPNETCORE_ENVIRONMENT not found in configuration");
            }

        }
    }

    public DbSet<Subscriber> Subscribers { get; set; }
    public DbSet<Phone> Phones { get; set; }
    public DbSet<Email> Emails { get; set; }
    public DbSet<ApiEndpoint> ApiEndpoints { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<Secret> Secrets { get; set; }
}
