using SirnakSport.Application.Common;
using SirnakSport.Application.DTOs.Facility;

namespace SirnakSport.Application.Interfaces;

/// <summary>
/// Tesis yönetimi servisi sözleşmesi.
/// </summary>
public interface IFacilityService
{
    Task<ServiceResult<IEnumerable<FacilityDto>>> GetAllAsync();
    Task<ServiceResult<FacilityDto>> GetByIdAsync(Guid id);
    Task<ServiceResult<FacilityDto>> CreateAsync(CreateFacilityDto dto);
    Task<ServiceResult<FacilityDto>> UpdateAsync(Guid id, UpdateFacilityDto dto);
    Task<ServiceResult> DeleteAsync(Guid id);
}
