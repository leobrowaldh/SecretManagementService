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

    public DbSet<Subscriber> Subscribers { get; set; }
    public DbSet<Phone> Phones { get; set; }
    public DbSet<Email> Emails { get; set; }
    public DbSet<ApiEndpoint> ApiEndpoints { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<Secret> Secrets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Secret>()
            .HasMany(s => s.Emails)
            .WithMany(e => e.Secrets)
            .UsingEntity(j => j.ToTable("EmailSecret", "suprusr"));

        modelBuilder.Entity<Secret>()
            .HasMany(s => s.Phones)
            .WithMany(e => e.Secrets)
            .UsingEntity(j => j.ToTable("PhoneSecret", "suprusr"));
    }

}
