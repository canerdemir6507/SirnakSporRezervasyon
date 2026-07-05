namespace SirnakSport.Application.DTOs.User;

/// <summary>
/// Kullanıcı bilgisi görüntüleme DTO'su.
/// </summary>
public class UserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
