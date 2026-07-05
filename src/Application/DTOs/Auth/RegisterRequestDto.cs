using System.ComponentModel.DataAnnotations;

namespace SirnakSport.Application.DTOs.Auth;

/// <summary>
/// Yeni kullanıcı kayıt isteği DTO'su.
/// </summary>
public class RegisterRequestDto
{
    [Required(ErrorMessage = "Ad soyad zorunludur.")]
    [MaxLength(100, ErrorMessage = "Ad soyad en fazla 100 karakter olabilir.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Öğrenci numarası zorunludur.")]
    [MaxLength(20, ErrorMessage = "Öğrenci numarası en fazla 20 karakter olabilir.")]
    public string StudentNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email adresi zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
    [MaxLength(150, ErrorMessage = "Email en fazla 150 karakter olabilir.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    public string Password { get; set; } = string.Empty;
}
