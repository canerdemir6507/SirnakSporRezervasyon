using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SirnakSport.Domain.Entities;
using SirnakSport.Domain.Enums;

namespace SirnakSport.Infrastructure.Data.Configurations;

/// <summary>
/// Reservation entity Fluent API yapılandırması.
/// </summary>
public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Date)
            .IsRequired();

        builder.Property(r => r.StartTime)
            .IsRequired();

        builder.Property(r => r.EndTime)
            .IsRequired();

        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(r => r.TotalPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        // Çakışma sorgularını optimize eden composite index
        builder.HasIndex(r => new { r.FacilityId, r.Date })
            .HasDatabaseName("IX_Reservations_FacilityId_Date");

        // Kullanıcı bazlı sorgular için index
        builder.HasIndex(r => r.UserId)
            .HasDatabaseName("IX_Reservations_UserId");
    }
}
