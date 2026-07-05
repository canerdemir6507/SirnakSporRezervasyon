using System.ComponentModel.DataAnnotations;

namespace SirnakSport.Application.DTOs.Auth;

/// <summary>
/// Kullanıcı giriş isteği DTO'su.
/// </summary>
public class LoginRequestDto
{
    [Required(ErrorMessage = "Email adresi zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    public string Password { get; set; } = string.Empty;
}
