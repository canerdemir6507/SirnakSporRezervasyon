namespace SirnakSport.Application.DTOs.ApprovedStudent;

/// <summary>
/// Onaylı öğrenci görüntüleme DTO'su.
/// </summary>
public class ApprovedStudentDto
{
    public Guid Id { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
