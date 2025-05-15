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
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Email>()
            .HasOne(e => e.Subscriber)
            .WithMany(s => s.Emails)
            .HasForeignKey(e => e.SubscriberId);

        modelBuilder.Entity<Email>()
            .HasMany(e => e.Applications)
            .WithMany(a => a.Emails)
            .UsingEntity(j => j.ToTable("EmailApplication", "suprusr"));

        modelBuilder.Entity<Phone>()
            .HasOne(p => p.Subscriber)
            .WithMany(s => s.Phones)
            .HasForeignKey(p => p.SubscriberId);

        modelBuilder.Entity<Phone>()
            .HasMany(p => p.Applications)
            .WithMany(a => a.Phones)
            .UsingEntity(j => j.ToTable("PhoneApplication", "suprusr"));

        //Indexing:
        modelBuilder.Entity<Application>()
            .HasIndex(a => a.ExternalApplicationId)
            .HasDatabaseName("IX_Applications_ExternalApplicationId");
    }

}
