namespace SirnakSport.Domain.Common;

/// <summary>
/// Tüm entity'ler için temel sınıf.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
