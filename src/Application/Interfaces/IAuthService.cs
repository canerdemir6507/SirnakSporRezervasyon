using SirnakSport.Application.Common;
using SirnakSport.Application.DTOs.Auth;

namespace SirnakSport.Application.Interfaces;

/// <summary>
/// Kimlik doğrulama servisi sözleşmesi.
/// </summary>
public interface IAuthService
{
    Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterRequestDto request);
    Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginRequestDto request);
}
