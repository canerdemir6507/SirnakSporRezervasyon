namespace SirnakSport.Application.DTOs.Auth;

/// <summary>
/// Kimlik doğrulama yanıt DTO'su — JWT token ve kullanıcı bilgilerini taşır.
/// </summary>
public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
