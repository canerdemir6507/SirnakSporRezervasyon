using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SirnakSport.Domain.Entities;
using SirnakSport.Domain.Enums;

namespace SirnakSport.Infrastructure.Data.Configurations;

/// <summary>
/// User entity Fluent API yapılandırması.
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.StudentNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        // Unique index'ler
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.StudentNumber).IsUnique();

        // Navigation
        builder.HasMany(u => u.Reservations)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
