using realtor_notes_backend.users.model;

namespace realtor_notes_backend.users.repository;
using Microsoft.EntityFrameworkCore;

public sealed class UsersDBContext : DbContext
{
    private readonly string _urlConnection;
    
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserSubscription> UserSubscriptions { get; set; } = null!;
    public DbSet<Session> Sessions { get; set; } = null!;
    
    public UsersDBContext(IConfiguration config)
    {
        Database.EnsureCreated();
        _urlConnection = config["SQLConnection"]!;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_urlConnection);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureUser(modelBuilder);
        ConfigureUserSubscription(modelBuilder);
        ConfigureSession(modelBuilder);
    }

    private void ConfigureSession(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Session>().HasKey(s => s.Id);
        modelBuilder.Entity<Session>().HasOne<User>()
            .WithOne().HasForeignKey<Session>(s => s.UserId);
        modelBuilder.Entity<Session>().Property(s => s.DeviceId)
            .HasMaxLength(16);
        modelBuilder.Entity<Session>().Property(s => s.DeviceName)
            .HasMaxLength(20);
        modelBuilder.Entity<Session>().Property(s => s.Platform)
            .HasMaxLength(20);
        modelBuilder.Entity<Session>().Property(s => s.Refresh)
            .HasMaxLength(16);
        modelBuilder.Entity<Session>().HasIndex(s => s.Refresh)
            .IsUnique();
    }

    private void ConfigureUserSubscription(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserSubscription>().HasKey(u => u.Id);
        modelBuilder.Entity<UserSubscription>().HasOne<User>()
            .WithOne().HasForeignKey<UserSubscription>(u => u.UserId);
        modelBuilder.Entity<UserSubscription>().HasOne(u => u.Subscription)
            .WithOne().HasForeignKey<UserSubscription>(u => u.SubscriptionId);
        modelBuilder.Entity<UserSubscription>().HasIndex(u => u.IsTrial);
    }

    private void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<User>().Property(u => u.Login)
            .IsRequired()
            .HasMaxLength(30);
        modelBuilder.Entity<User>().Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(30);
        modelBuilder.Entity<User>().Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(64);
        modelBuilder.Entity<User>().Property(u => u.TwoFaSecret)
            .IsRequired()
            .HasMaxLength(16);
        modelBuilder.Entity<User>().HasIndex(u => u.Login)
            .IsUnique();
        modelBuilder.Entity<User>().HasIndex(u => u.Email)
            .IsUnique();
    }
}