using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SirnakSport.Domain.Entities;

namespace SirnakSport.Infrastructure.Data.Configurations;

/// <summary>
/// ApprovedStudent entity Fluent API yapılandırması.
/// </summary>
public class ApprovedStudentConfiguration : IEntityTypeConfiguration<ApprovedStudent>
{
    public void Configure(EntityTypeBuilder<ApprovedStudent> builder)
    {
        builder.ToTable("ApprovedStudents");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.StudentNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(a => a.FullName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Department)
            .HasMaxLength(150);

        builder.Property(a => a.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Unique index — aynı öğrenci numarası tekrar eklenemez
        builder.HasIndex(a => a.StudentNumber)
            .IsUnique()
            .HasDatabaseName("IX_ApprovedStudents_StudentNumber");
    }
}
