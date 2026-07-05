using SirnakSport.Application.Common;
using SirnakSport.Application.DTOs.User;

namespace SirnakSport.Application.Interfaces;

/// <summary>
/// Kullanıcı yönetimi servisi sözleşmesi (Admin işlemleri).
/// </summary>
public interface IUserService
{
    Task<ServiceResult<IEnumerable<UserDto>>> GetAllAsync();
    Task<ServiceResult<UserDto>> GetByIdAsync(Guid id);
}
