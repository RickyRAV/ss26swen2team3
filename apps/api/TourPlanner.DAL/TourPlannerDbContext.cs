using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TourPlanner.Models;

namespace TourPlanner.DAL;

public class TourPlannerDbContext : IdentityDbContext<ApplicationUser>
{
    public TourPlannerDbContext(DbContextOptions<TourPlannerDbContext> options) : base(options) { }

    public DbSet<Tour> Tours => Set<Tour>();
    public DbSet<TourLog> TourLogs => Set<TourLog>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Tour>(t =>
        {
            t.HasKey(x => x.Id);
            t.Property(x => x.Name).IsRequired().HasMaxLength(200);
            t.Property(x => x.From).IsRequired().HasMaxLength(300);
            t.Property(x => x.To).IsRequired().HasMaxLength(300);
            t.Property(x => x.Description).HasMaxLength(2000);
            t.Property(x => x.RouteInformation).HasColumnType("text");
            t.Property(x => x.TransportType).HasConversion<string>();

            // Computed properties are not mapped to columns
            t.Ignore(x => x.Popularity);
            t.Ignore(x => x.ChildFriendliness);

            t.HasOne<ApplicationUser>()
                .WithMany(u => u.Tours)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            t.HasMany(x => x.TourLogs)
                .WithOne(l => l.Tour)
                .HasForeignKey(l => l.TourId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<TourLog>(l =>
        {
            l.HasKey(x => x.Id);
            l.Property(x => x.Comment).HasMaxLength(2000);
            l.Property(x => x.Difficulty).HasConversion<string>();
            l.Property(x => x.Rating).HasConversion<string>();
        });

        builder.Entity<RefreshToken>(r =>
        {
            r.HasKey(x => x.Id);
            r.HasIndex(x => x.Token).IsUnique();
            r.HasIndex(x => x.UserId);
        });
    }
}
