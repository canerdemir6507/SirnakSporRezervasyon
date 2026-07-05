using System.ComponentModel.DataAnnotations;

namespace SirnakSport.Application.DTOs.ApprovedStudent;

/// <summary>
/// Onaylı öğrenci ekleme DTO'su.
/// </summary>
public class CreateApprovedStudentDto
{
    [Required(ErrorMessage = "Öğrenci numarası zorunludur.")]
    public string StudentNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ad soyad zorunludur.")]
    [MaxLength(100, ErrorMessage = "Ad soyad en fazla 100 karakter olabilir.")]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(150, ErrorMessage = "Bölüm adı en fazla 150 karakter olabilir.")]
    public string Department { get; set; } = string.Empty;
}
