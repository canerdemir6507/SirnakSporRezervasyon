using SirnakSport.Domain.Common;

namespace SirnakSport.Domain.Entities;

/// <summary>
/// Spor tesisi entity'si.
/// </summary>
public class Facility : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal HourlyPrice { get; set; }
    public TimeSpan OpenTime { get; set; }
    public TimeSpan CloseTime { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation property
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
