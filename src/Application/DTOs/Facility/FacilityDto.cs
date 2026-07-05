namespace SirnakSport.Application.DTOs.Facility;

/// <summary>
/// Tesis bilgisi görüntüleme DTO'su.
/// </summary>
public class FacilityDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal HourlyPrice { get; set; }
    public string OpenTime { get; set; } = string.Empty;
    public string CloseTime { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
