using System.ComponentModel.DataAnnotations;

namespace SirnakSport.Application.DTOs.Reservation;

/// <summary>
/// Rezervasyon oluşturma isteği DTO'su.
/// </summary>
public class CreateReservationDto
{
    [Required(ErrorMessage = "Tesis ID zorunludur.")]
    public Guid FacilityId { get; set; }

    [Required(ErrorMessage = "Tarih zorunludur. (yyyy-MM-dd formatında)")]
    public string Date { get; set; } = string.Empty;

    [Required(ErrorMessage = "Başlangıç saati zorunludur. (HH:mm formatında)")]
    public string StartTime { get; set; } = string.Empty;

    [Required(ErrorMessage = "Bitiş saati zorunludur. (HH:mm formatında)")]
    public string EndTime { get; set; } = string.Empty;
}
