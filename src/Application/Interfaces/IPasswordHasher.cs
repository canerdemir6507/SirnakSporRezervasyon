namespace SirnakSport.Application.Interfaces;

/// <summary>
/// Şifre hashleme servisi sözleşmesi.
/// </summary>
public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
