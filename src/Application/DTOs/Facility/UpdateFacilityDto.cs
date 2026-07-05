using System.ComponentModel.DataAnnotations;

namespace SirnakSport.Application.DTOs.Facility;

/// <summary>
/// Tesis güncelleme DTO'su.
/// </summary>
public class UpdateFacilityDto
{
    [Required(ErrorMessage = "Tesis adı zorunludur.")]
    [MaxLength(100, ErrorMessage = "Tesis adı en fazla 100 karakter olabilir.")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Saatlik ücret zorunludur.")]
    [Range(0, double.MaxValue, ErrorMessage = "Saatlik ücret 0 veya daha büyük olmalıdır.")]
    public decimal HourlyPrice { get; set; }

    [Required(ErrorMessage = "Açılış saati zorunludur. (HH:mm formatında)")]
    public string OpenTime { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kapanış saati zorunludur. (HH:mm formatında)")]
    public string CloseTime { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
