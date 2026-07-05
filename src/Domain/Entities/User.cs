using SirnakSport.Domain.Common;
using SirnakSport.Domain.Enums;

namespace SirnakSport.Domain.Entities;

/// <summary>
/// Kullanıcı (öğrenci veya admin) entity'si.
/// </summary>
public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;

    // Navigation property
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
