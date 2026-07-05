using Microsoft.EntityFrameworkCore;
using SirnakSport.Domain.Entities;

namespace SirnakSport.Infrastructure.Data;

/// <summary>
/// Entity Framework Core veritabanı context'i.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Facility> Facilities => Set<Facility>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<ApprovedStudent> ApprovedStudents => Set<ApprovedStudent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Fluent API configurations uygula
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // UpdatedAt alanını otomatik güncelle
        foreach (var entry in ChangeTracker.Entries<Domain.Common.BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
