using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SirnakSport.Domain.Entities;
using SirnakSport.Domain.Enums;

namespace SirnakSport.Infrastructure.Data.Seed;

/// <summary>
/// Veritabanı başlangıç verilerini oluşturur.
/// 5 spor tesisi + 1 admin kullanıcı + örnek onaylı öğrenciler seed eder.
/// </summary>
public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        try
        {
            // Veritabanını yeniden oluştur (yeni tablo yapısı için)
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            await SeedFacilitiesAsync(context, logger);
            await SeedAdminUserAsync(context, logger);
            await SeedApprovedStudentsAsync(context, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Seed data oluşturulurken bir hata oluştu.");
            throw;
        }
    }

    private static async Task SeedFacilitiesAsync(AppDbContext context, ILogger logger)
    {
        if (await context.Facilities.AnyAsync())
        {
            logger.LogInformation("Tesisler zaten mevcut, seed atlanıyor.");
            return;
        }

        var facilities = new List<Facility>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Halı Saha 1",
                Description = "Şırnak Üniversitesi ana halı saha tesisi. 7 kişilik takımlar için uygun, sentetik çim zemin.",
                HourlyPrice = 450m,
                OpenTime = new TimeSpan(8, 0, 0),
                CloseTime = new TimeSpan(22, 0, 0),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Halı Saha 2",
                Description = "Şırnak Üniversitesi yardımcı halı saha tesisi. 5 kişilik takımlar için uygun, sentetik çim zemin.",
                HourlyPrice = 450m,
                OpenTime = new TimeSpan(8, 0, 0),
                CloseTime = new TimeSpan(22, 0, 0),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Voleybol Sahası",
                Description = "Açık hava voleybol sahası. Profesyonel file ve zemin döşemesi ile donatılmıştır.",
                HourlyPrice = 300m,
                OpenTime = new TimeSpan(9, 0, 0),
                CloseTime = new TimeSpan(21, 0, 0),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Basketbol Sahası",
                Description = "Kapalı basketbol sahası. NBA standartlarında pota ve parke zemin.",
                HourlyPrice = 300m,
                OpenTime = new TimeSpan(9, 0, 0),
                CloseTime = new TimeSpan(21, 0, 0),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Fitness Salonu",
                Description = "Modern fitness ekipmanları ile donatılmış spor salonu. Kardio ve ağırlık alanları mevcuttur. Günlük giriş ücreti.",
                HourlyPrice = 30m,
                OpenTime = new TimeSpan(7, 0, 0),
                CloseTime = new TimeSpan(23, 0, 0),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.Facilities.AddRangeAsync(facilities);
        await context.SaveChangesAsync();

        logger.LogInformation("{Count} tesis başarıyla seed edildi.", facilities.Count);
    }

    private static async Task SeedAdminUserAsync(AppDbContext context, ILogger logger)
    {
        if (await context.Users.AnyAsync(u => u.Role == UserRole.Admin))
        {
            logger.LogInformation("Admin kullanıcı zaten mevcut, seed atlanıyor.");
            return;
        }

        var admin = new User
        {
            Id = Guid.NewGuid(),
            FullName = "Sistem Yöneticisi",
            StudentNumber = "ADMIN001",
            Email = "admin@sirnak.edu.tr",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Role = UserRole.Admin,
            CreatedAt = DateTime.UtcNow
        };

        await context.Users.AddAsync(admin);
        await context.SaveChangesAsync();

        logger.LogInformation("Admin kullanıcı başarıyla oluşturuldu: {Email}", admin.Email);
    }

    private static async Task SeedApprovedStudentsAsync(AppDbContext context, ILogger logger)
    {
        if (await context.ApprovedStudents.AnyAsync())
        {
            logger.LogInformation("Onaylı öğrenciler zaten mevcut, seed atlanıyor.");
            return;
        }

        var approvedStudents = new List<ApprovedStudent>
        {
            new()
            {
                Id = Guid.NewGuid(),
                StudentNumber = "210101001",
                FullName = "Ahmet Yılmaz",
                Department = "Bilgisayar Mühendisliği",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                StudentNumber = "210102002",
                FullName = "Fatma Kaya",
                Department = "Elektrik-Elektronik Mühendisliği",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                StudentNumber = "210103003",
                FullName = "Mehmet Demir",
                Department = "İnşaat Mühendisliği",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                StudentNumber = "210201004",
                FullName = "Ayşe Çelik",
                Department = "Tıp Fakültesi",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                StudentNumber = "210301005",
                FullName = "Ali Öztürk",
                Department = "Hukuk Fakültesi",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.ApprovedStudents.AddRangeAsync(approvedStudents);
        await context.SaveChangesAsync();

        logger.LogInformation("{Count} onaylı öğrenci başarıyla seed edildi.", approvedStudents.Count);
    }
}
