using SirnakSport.Domain.Common;

namespace SirnakSport.Domain.Entities;

/// <summary>
/// Onaylı öğrenci kaydı — sadece bu listede yer alan öğrenci numaraları sisteme kayıt olabilir.
/// Admin tarafından yönetilir (whitelist).
/// </summary>
public class ApprovedStudent : BaseEntity
{
    public string StudentNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
