namespace SirnakSport.Application.DTOs.Reservation;

/// <summary>
/// Müsait zaman dilimi bilgisi DTO'su.
/// </summary>
public class AvailableSlotDto
{
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
}
