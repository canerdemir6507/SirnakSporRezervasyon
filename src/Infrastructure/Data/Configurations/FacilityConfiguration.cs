using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SirnakSport.Domain.Entities;

namespace SirnakSport.Infrastructure.Data.Configurations;

/// <summary>
/// Facility entity Fluent API yapılandırması.
/// </summary>
public class FacilityConfiguration : IEntityTypeConfiguration<Facility>
{
    public void Configure(EntityTypeBuilder<Facility> builder)
    {
        builder.ToTable("Facilities");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.Description)
            .HasMaxLength(500);

        builder.Property(f => f.HourlyPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(f => f.OpenTime)
            .IsRequired();

        builder.Property(f => f.CloseTime)
            .IsRequired();

        builder.Property(f => f.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Navigation
        builder.HasMany(f => f.Reservations)
            .WithOne(r => r.Facility)
            .HasForeignKey(r => r.FacilityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
