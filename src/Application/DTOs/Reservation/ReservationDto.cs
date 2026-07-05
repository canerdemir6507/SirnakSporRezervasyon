namespace SirnakSport.Application.DTOs.Reservation;

/// <summary>
/// Rezervasyon bilgisi görüntüleme DTO'su.
/// </summary>
public class ReservationDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public string UserStudentNumber { get; set; } = string.Empty;
    public Guid FacilityId { get; set; }
    public string FacilityName { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
}
