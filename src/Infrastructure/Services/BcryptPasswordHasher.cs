using SirnakSport.Application.Interfaces;

namespace SirnakSport.Infrastructure.Services;

/// <summary>
/// BCrypt şifre hashleme servisi implementasyonu.
/// </summary>
public class BcryptPasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
