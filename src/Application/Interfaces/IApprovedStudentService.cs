using SirnakSport.Application.Common;
using SirnakSport.Application.DTOs.ApprovedStudent;

namespace SirnakSport.Application.Interfaces;

/// <summary>
/// Onaylı öğrenci yönetimi servisi sözleşmesi (Admin whitelist).
/// </summary>
public interface IApprovedStudentService
{
    Task<ServiceResult<IEnumerable<ApprovedStudentDto>>> GetAllAsync();
    Task<ServiceResult<ApprovedStudentDto>> GetByStudentNumberAsync(string studentNumber);
    Task<ServiceResult<ApprovedStudentDto>> CreateAsync(CreateApprovedStudentDto dto);
    Task<ServiceResult<IEnumerable<ApprovedStudentDto>>> BulkCreateAsync(BulkCreateApprovedStudentDto dto);
    Task<ServiceResult> DeactivateAsync(Guid id);
    Task<ServiceResult> ActivateAsync(Guid id);
    Task<ServiceResult> DeleteAsync(Guid id);
}
