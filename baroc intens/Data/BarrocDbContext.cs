using System;
using System.IO;
using baroc_intens.Models;
using Microsoft.EntityFrameworkCore;

namespace baroc_intens.Data;

public sealed class BarrocDbContext : DbContext
{
    public BarrocDbContext()
        : base(CreateOptions())
    {
    }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<IncidentRequest> IncidentRequests => Set<IncidentRequest>();

    public static string DatabasePath
    {
        get
        {
            var directory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BarrocIntens");
            Directory.CreateDirectory(directory);
            return Path.Combine(directory, "barroc-intens.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>()
            .HasIndex(user => user.Username)
            .IsUnique();
        modelBuilder.Entity<AppUser>().Property(user => user.Username).HasMaxLength(80);
        modelBuilder.Entity<AppUser>().Property(user => user.DisplayName).HasMaxLength(120);
        modelBuilder.Entity<AppUser>().Property(user => user.Role).HasConversion<string>();

        modelBuilder.Entity<Customer>().Property(customer => customer.Name).HasMaxLength(160);
        modelBuilder.Entity<Customer>().Property(customer => customer.ContactName).HasMaxLength(120);

        modelBuilder.Entity<IncidentRequest>().Property(request => request.Title).HasMaxLength(120);
        modelBuilder.Entity<IncidentRequest>().Property(request => request.Description).HasMaxLength(2000);
        modelBuilder.Entity<IncidentRequest>().Property(request => request.Location).HasMaxLength(200);
        modelBuilder.Entity<IncidentRequest>().Property(request => request.Status).HasConversion<string>();
        modelBuilder.Entity<IncidentRequest>()
            .HasOne(request => request.Customer)
            .WithMany()
            .HasForeignKey(request => request.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<IncidentRequest>()
            .HasOne(request => request.CreatedByUser)
            .WithMany()
            .HasForeignKey(request => request.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static DbContextOptions<BarrocDbContext> CreateOptions()
    {
        return new DbContextOptionsBuilder<BarrocDbContext>()
            .UseSqlite($"Data Source={DatabasePath}")
            .Options;
    }
}
