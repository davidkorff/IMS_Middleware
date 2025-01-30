using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ImsMonitoring.Models;

namespace ImsMonitoring.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Submission> Submissions { get; set; } = null!;
    public DbSet<ImsInstance> ImsInstances { get; set; }
    public DbSet<ExternalSystem> ExternalSystems { get; set; }
    public DbSet<ImsInstanceConnection> ImsInstanceConnections { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>()
            .HasIndex(u => u.Email)
            .IsUnique();

        builder.Entity<ImsInstance>()
            .HasOne(i => i.User)
            .WithMany(u => u.ImsInstances)
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ImsInstanceConnection>()
            .HasOne(ic => ic.ImsInstance)
            .WithMany(i => i.Connections)
            .HasForeignKey(ic => ic.ImsInstanceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ImsInstanceConnection>()
            .HasOne(ic => ic.ExternalSystem)
            .WithMany(es => es.Connections)
            .HasForeignKey(ic => ic.ExternalSystemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ExternalSystem>().HasData(
            new ExternalSystem 
            { 
                Id = Guid.NewGuid(),
                Name = "AIM",
                Version = "1.0",
                Description = "AIM Invoicing Platform v1",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new ExternalSystem 
            { 
                Id = Guid.NewGuid(),
                Name = "AIM",
                Version = "2.0",
                Description = "AIM Invoicing Platform v2",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        );
    }
} 