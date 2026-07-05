using System.ComponentModel.DataAnnotations;

namespace SirnakSport.Application.DTOs.ApprovedStudent;

/// <summary>
/// Toplu onaylı öğrenci ekleme DTO'su.
/// Admin birden fazla öğrenciyi tek seferde ekleyebilir.
/// </summary>
public class BulkCreateApprovedStudentDto
{
    [Required(ErrorMessage = "Öğrenci listesi boş olamaz.")]
    public List<CreateApprovedStudentDto> Students { get; set; } = new();
}
