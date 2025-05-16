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
    public DbSet<EmailApplication> EmailApplications { get; set; }
    public DbSet<PhoneApplication> PhoneApplications { get; set; }
    public DbSet<SubscriberUser> SubscriberUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //One to Many relationships:
        modelBuilder.Entity<Email>()
            .HasOne(e => e.Subscriber)
            .WithMany(s => s.Emails)
            .HasForeignKey(e => e.SubscriberId);

        modelBuilder.Entity<Phone>()
            .HasOne(p => p.Subscriber)
            .WithMany(s => s.Phones)
            .HasForeignKey(p => p.SubscriberId);

        //Many to Many relationships:
        modelBuilder.Entity<EmailApplication>()
            .HasOne(ea => ea.Email)
            .WithMany(e => e.EmailApplications)
            .HasForeignKey(ea => ea.EmailId);

        modelBuilder.Entity<EmailApplication>()
            .HasOne(ea => ea.Application)
            .WithMany(a => a.EmailApplications)
            .HasForeignKey(ea => ea.ApplicationId);

        modelBuilder.Entity<PhoneApplication>()
            .HasOne(pa => pa.Phone)
            .WithMany(p => p.PhoneApplications)
            .HasForeignKey(pa => pa.PhoneId);

        modelBuilder.Entity<PhoneApplication>()
            .HasOne(pa => pa.Application)
            .WithMany(a => a.PhoneApplications)
            .HasForeignKey(pa => pa.ApplicationId);

        modelBuilder.Entity<SubscriberUser>()
            .HasOne(su => su.User)
            .WithMany(u => u.SubscriberUsers)
            .HasForeignKey(su => su.UserId);

        modelBuilder.Entity<SubscriberUser>()
            .HasOne(su => su.Subscriber)
            .WithMany(s => s.SubscriberUsers)
            .HasForeignKey(su => su.SubscriberId);

        //Joining Tables:
        modelBuilder.Entity<EmailApplication>()
        .HasKey(ea => new { ea.EmailId, ea.ApplicationId });

        modelBuilder.Entity<PhoneApplication>()
            .HasKey(pa => new { pa.PhoneId, pa.ApplicationId });

        modelBuilder.Entity<SubscriberUser>()
            .HasKey(su => new { su.UserId, su.SubscriberId });

        //Indexing:
        modelBuilder.Entity<Application>()
            .HasIndex(a => a.ExternalApplicationId)
            .HasDatabaseName("IX_Applications_ExternalApplicationId");
    }

}
