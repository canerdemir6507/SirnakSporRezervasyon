using SirnakSport.Domain.Common;
using SirnakSport.Domain.Enums;

namespace SirnakSport.Domain.Entities;

/// <summary>
/// Rezervasyon entity'si.
/// </summary>
public class Reservation : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid FacilityId { get; set; }
    public DateOnly Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public decimal TotalPrice { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public Facility Facility { get; set; } = null!;
}
