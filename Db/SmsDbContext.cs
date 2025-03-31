using Db.DbModels;
using Microsoft.EntityFrameworkCore;
using System;

namespace Db;

public class SmsDbContext: DbContext
{
    public SmsDbContext(DbContextOptions<SmsDbContext> options) : base(options) { }
    public DbSet<Subscriber> Subscribers { get; set; }
    public DbSet<Phone> Phones { get; set; }
    public DbSet<Email> Emails { get; set; }
    public DbSet<ApiEndpoint> ApiEndpoints { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<Secret> Secrets { get; set; }
}
