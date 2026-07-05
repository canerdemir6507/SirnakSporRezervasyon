using SirnakSport.Domain.Entities;

namespace SirnakSport.Domain.Interfaces;

/// <summary>
/// Onaylı öğrenci repository sözleşmesi.
/// </summary>
public interface IApprovedStudentRepository : IRepository<ApprovedStudent>
{
    Task<bool> IsStudentApprovedAsync(string studentNumber);
    Task<ApprovedStudent?> GetByStudentNumberAsync(string studentNumber);
    Task<IEnumerable<ApprovedStudent>> GetActiveStudentsAsync();
}
