using SirnakSport.Domain.Entities;

namespace SirnakSport.Application.Interfaces;

/// <summary>
/// JWT token üretim servisi sözleşmesi.
/// </summary>
public interface IJwtTokenService
{
    string GenerateToken(User user);
    DateTime GetExpiration();
}
