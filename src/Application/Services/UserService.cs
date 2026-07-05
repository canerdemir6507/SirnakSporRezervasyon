using AutoMapper;
using SirnakSport.Application.Common;
using SirnakSport.Application.DTOs.User;
using SirnakSport.Application.Interfaces;
using SirnakSport.Domain.Interfaces;

namespace SirnakSport.Application.Services;

/// <summary>
/// Kullanıcı yönetimi servisi implementasyonu.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<IEnumerable<UserDto>>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        var dtos = _mapper.Map<IEnumerable<UserDto>>(users);
        return ServiceResult<IEnumerable<UserDto>>.Success(dtos);
    }

    public async Task<ServiceResult<UserDto>> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return ServiceResult<UserDto>.Failure("Kullanıcı bulunamadı.", 404);
        }

        var dto = _mapper.Map<UserDto>(user);
        return ServiceResult<UserDto>.Success(dto);
    }
}
