using Microsoft.EntityFrameworkCore;
using OrbitalSOS.Domain.Entities;
namespace OrbitalSOS.Infrastructure.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<User> Users => Set<User>();
    public DbSet<Shelter> Shelters => Set<Shelter>();
    public DbSet<ClimateAlert> ClimateAlerts => Set<ClimateAlert>();
    public DbSet<RiskArea> RiskAreas => Set<RiskArea>();
    public DbSet<SosRequest> SosRequests => Set<SosRequest>();
    public DbSet<Donation> Donations => Set<Donation>();
    public DbSet<ShelterNeed> ShelterNeeds => Set<ShelterNeed>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e => {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Name).IsRequired().HasMaxLength(200);
            e.Property(u => u.Email).IsRequired().HasMaxLength(200);
        });
        modelBuilder.Entity<Shelter>(e => {
            e.HasKey(s => s.Id);
            e.HasIndex(s => new { s.Latitude, s.Longitude });
            e.Property(s => s.Name).IsRequired().HasMaxLength(200);
            e.HasMany(s => s.SosRequests).WithOne(r => r.SuggestedShelter).HasForeignKey(r => r.SuggestedShelterId).OnDelete(DeleteBehavior.SetNull);
        });
        modelBuilder.Entity<ClimateAlert>(e => {
            e.HasKey(c => c.Id);
            e.HasIndex(c => c.AffectedRegion);
            e.HasIndex(c => c.IsActive);
        });
        modelBuilder.Entity<RiskArea>(e => {
            e.HasKey(r => r.Id);
            e.HasOne(r => r.ClimateAlert).WithMany(c => c.RiskAreas).HasForeignKey(r => r.ClimateAlertId).OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<SosRequest>(e => {
            e.HasKey(r => r.Id);
            e.HasIndex(r => r.Status);
            e.HasOne(r => r.User).WithMany(u => u.SosRequests).HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Donation>(e => {
            e.HasKey(d => d.Id);
            e.HasIndex(d => d.ShelterId);
            e.HasOne(d => d.Shelter).WithMany(s => s.Donations).HasForeignKey(d => d.ShelterId).OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<ShelterNeed>(e => {
            e.HasKey(n => n.Id);
            e.HasIndex(n => n.ShelterId);
            e.HasOne(n => n.Shelter).WithMany(s => s.ShelterNeeds).HasForeignKey(n => n.ShelterId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
