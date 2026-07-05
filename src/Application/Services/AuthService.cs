using SirnakSport.Application.Common;
using SirnakSport.Application.DTOs.Auth;
using SirnakSport.Application.Interfaces;
using SirnakSport.Application.Validators;
using SirnakSport.Domain.Entities;
using SirnakSport.Domain.Enums;
using SirnakSport.Domain.Interfaces;

namespace SirnakSport.Application.Services;

/// <summary>
/// Kimlik doğrulama servisi implementasyonu.
/// Kullanıcı kayıt ve giriş işlemlerini yönetir.
/// Kayıt sırasında öğrenci numarası format doğrulaması ve whitelist kontrolü yapar.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IApprovedStudentRepository _approvedStudentRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(
        IUserRepository userRepository,
        IApprovedStudentRepository approvedStudentRepository,
        IJwtTokenService jwtTokenService,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _approvedStudentRepository = approvedStudentRepository;
        _jwtTokenService = jwtTokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterRequestDto request)
    {
        // ─── 1. Öğrenci numarası FORMAT kontrolü ───
        var (isValid, validationError) = StudentNumberValidator.Validate(request.StudentNumber);
        if (!isValid)
        {
            return ServiceResult<AuthResponseDto>.Failure(validationError!);
        }

        // ─── 2. Whitelist kontrolü — sadece onaylı öğrenciler kayıt olabilir ───
        var isApproved = await _approvedStudentRepository.IsStudentApprovedAsync(request.StudentNumber.Trim());
        if (!isApproved)
        {
            return ServiceResult<AuthResponseDto>.Failure(
                "Bu öğrenci numarası onaylı öğrenci listesinde bulunamadı. " +
                "Lütfen üniversite yönetimiyle iletişime geçiniz.", 403);
        }

        // ─── 3. Duplicate kontrolleri ───
        if (await _userRepository.EmailExistsAsync(request.Email))
        {
            return ServiceResult<AuthResponseDto>.Failure(
                "Bu email adresi zaten kullanılmaktadır.", 409);
        }

        if (await _userRepository.StudentNumberExistsAsync(request.StudentNumber))
        {
            return ServiceResult<AuthResponseDto>.Failure(
                "Bu öğrenci numarası zaten kayıtlıdır.", 409);
        }

        // ─── 4. Yeni kullanıcı oluştur ───
        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            StudentNumber = request.StudentNumber.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            Role = UserRole.User,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        // Token üret ve yanıt döndür
        var token = _jwtTokenService.GenerateToken(user);
        var response = new AuthResponseDto
        {
            Token = token,
            Expiration = _jwtTokenService.GetExpiration(),
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString()
        };

        return ServiceResult<AuthResponseDto>.Success(response, 201);
    }

    public async Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginRequestDto request)
    {
        // Kullanıcıyı email ile bul
        var user = await _userRepository.GetByEmailAsync(request.Email.Trim().ToLowerInvariant());
        if (user == null)
        {
            return ServiceResult<AuthResponseDto>.Failure(
                "Email veya şifre hatalı.", 401);
        }

        // Şifre doğrula
        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return ServiceResult<AuthResponseDto>.Failure(
                "Email veya şifre hatalı.", 401);
        }

        // Token üret
        var token = _jwtTokenService.GenerateToken(user);
        var response = new AuthResponseDto
        {
            Token = token,
            Expiration = _jwtTokenService.GetExpiration(),
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString()
        };

        return ServiceResult<AuthResponseDto>.Success(response);
    }
}
