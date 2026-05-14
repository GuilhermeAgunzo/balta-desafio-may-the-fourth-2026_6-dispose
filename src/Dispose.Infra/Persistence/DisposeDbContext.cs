using Dispose.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dispose.Infra.Persistence;

public sealed class DisposeDbContext(DbContextOptions<DisposeDbContext> options) : DbContext(options)
{
    public DbSet<Neighborhood> Neighborhoods => Set<Neighborhood>();
    public DbSet<CollectionSchedule> CollectionSchedules => Set<CollectionSchedule>();
    public DbSet<CollectionPoint> CollectionPoints => Set<CollectionPoint>();
    public DbSet<CollectionPointAcceptedCategory> CollectionPointAcceptedCategories => Set<CollectionPointAcceptedCategory>();
    public DbSet<ReminderSubscription> ReminderSubscriptions => Set<ReminderSubscription>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Neighborhood>(entity =>
        {
            entity.ToTable("Neighborhoods");
            entity.HasKey(neighborhood => neighborhood.Id);
            entity.Property(neighborhood => neighborhood.Name).HasMaxLength(140).IsRequired();
            entity.Property(neighborhood => neighborhood.Sector).HasMaxLength(120).IsRequired();
            entity.Property(neighborhood => neighborhood.Motto).HasMaxLength(220).IsRequired();
            entity.HasIndex(neighborhood => neighborhood.Name).IsUnique();
        });

        modelBuilder.Entity<CollectionSchedule>(entity =>
        {
            entity.ToTable("CollectionSchedules");
            entity.HasKey(schedule => schedule.Id);
            entity.Property(schedule => schedule.PickupWindow).HasMaxLength(80).IsRequired();
            entity.Property(schedule => schedule.RouteCode).HasMaxLength(40).IsRequired();
            entity.Property(schedule => schedule.Guidance).HasMaxLength(320).IsRequired();
            entity.HasOne(schedule => schedule.Neighborhood)
                .WithMany(neighborhood => neighborhood.CollectionSchedules)
                .HasForeignKey(schedule => schedule.NeighborhoodId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CollectionPoint>(entity =>
        {
            entity.ToTable("CollectionPoints");
            entity.HasKey(point => point.Id);
            entity.Property(point => point.Name).HasMaxLength(160).IsRequired();
            entity.Property(point => point.NeighborhoodName).HasMaxLength(140).IsRequired();
            entity.Property(point => point.Address).HasMaxLength(220).IsRequired();
            entity.Property(point => point.Landmark).HasMaxLength(220).IsRequired();
            entity.Property(point => point.FactionTag).HasMaxLength(80).IsRequired();
        });

        modelBuilder.Entity<CollectionPointAcceptedCategory>(entity =>
        {
            entity.ToTable("CollectionPointAcceptedCategories");
            entity.HasKey(category => new { category.CollectionPointId, category.Category });
            entity.HasOne(category => category.CollectionPoint)
                .WithMany(point => point.AcceptedCategories)
                .HasForeignKey(category => category.CollectionPointId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ReminderSubscription>(entity =>
        {
            entity.ToTable("ReminderSubscriptions");
            entity.HasKey(reminder => reminder.Id);
            entity.Property(reminder => reminder.ResidentAlias).HasMaxLength(120).IsRequired();
            entity.Property(reminder => reminder.CollectionPointName).HasMaxLength(160).IsRequired();
            entity.Property(reminder => reminder.CollectionPointAddress).HasMaxLength(220).IsRequired();
            entity.HasIndex(reminder => reminder.CollectionPointId);
        });
    }
}
