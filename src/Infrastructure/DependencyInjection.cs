using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SirnakSport.Application.Interfaces;
using SirnakSport.Application.Services;
using SirnakSport.Domain.Interfaces;
using SirnakSport.Infrastructure.Data;
using SirnakSport.Infrastructure.Repositories;
using SirnakSport.Infrastructure.Services;

namespace SirnakSport.Infrastructure;

/// <summary>
/// Infrastructure katmanı dependency injection kayıtları.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IFacilityRepository, FacilityRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IApprovedStudentRepository, ApprovedStudentRepository>();

        // Infrastructure Services
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

        // Application Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IFacilityService, FacilityService>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IApprovedStudentService, ApprovedStudentService>();

        return services;
    }
}
