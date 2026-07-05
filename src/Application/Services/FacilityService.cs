using AutoMapper;
using SirnakSport.Application.Common;
using SirnakSport.Application.DTOs.Facility;
using SirnakSport.Application.Interfaces;
using SirnakSport.Domain.Entities;
using SirnakSport.Domain.Interfaces;

namespace SirnakSport.Application.Services;

/// <summary>
/// Tesis yönetimi servisi implementasyonu.
/// Admin tesis CRUD işlemleri ve tesis listeleme.
/// </summary>
public class FacilityService : IFacilityService
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly IMapper _mapper;

    public FacilityService(IFacilityRepository facilityRepository, IMapper mapper)
    {
        _facilityRepository = facilityRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<IEnumerable<FacilityDto>>> GetAllAsync()
    {
        var facilities = await _facilityRepository.GetActiveFacilitiesAsync();
        var dtos = _mapper.Map<IEnumerable<FacilityDto>>(facilities);
        return ServiceResult<IEnumerable<FacilityDto>>.Success(dtos);
    }

    public async Task<ServiceResult<FacilityDto>> GetByIdAsync(Guid id)
    {
        var facility = await _facilityRepository.GetByIdAsync(id);
        if (facility == null)
        {
            return ServiceResult<FacilityDto>.Failure("Tesis bulunamadı.", 404);
        }

        var dto = _mapper.Map<FacilityDto>(facility);
        return ServiceResult<FacilityDto>.Success(dto);
    }

    public async Task<ServiceResult<FacilityDto>> CreateAsync(CreateFacilityDto dto)
    {
        if (!TimeSpan.TryParse(dto.OpenTime, out var openTime))
        {
            return ServiceResult<FacilityDto>.Failure("Geçersiz açılış saati formatı. HH:mm formatında giriniz.");
        }

        if (!TimeSpan.TryParse(dto.CloseTime, out var closeTime))
        {
            return ServiceResult<FacilityDto>.Failure("Geçersiz kapanış saati formatı. HH:mm formatında giriniz.");
        }

        if (openTime >= closeTime)
        {
            return ServiceResult<FacilityDto>.Failure("Açılış saati kapanış saatinden önce olmalıdır.");
        }

        var facility = new Facility
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim() ?? string.Empty,
            HourlyPrice = dto.HourlyPrice,
            OpenTime = openTime,
            CloseTime = closeTime,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _facilityRepository.AddAsync(facility);

        var resultDto = _mapper.Map<FacilityDto>(facility);
        return ServiceResult<FacilityDto>.Success(resultDto, 201);
    }

    public async Task<ServiceResult<FacilityDto>> UpdateAsync(Guid id, UpdateFacilityDto dto)
    {
        var facility = await _facilityRepository.GetByIdAsync(id);
        if (facility == null)
        {
            return ServiceResult<FacilityDto>.Failure("Tesis bulunamadı.", 404);
        }

        if (!TimeSpan.TryParse(dto.OpenTime, out var openTime))
        {
            return ServiceResult<FacilityDto>.Failure("Geçersiz açılış saati formatı. HH:mm formatında giriniz.");
        }

        if (!TimeSpan.TryParse(dto.CloseTime, out var closeTime))
        {
            return ServiceResult<FacilityDto>.Failure("Geçersiz kapanış saati formatı. HH:mm formatında giriniz.");
        }

        if (openTime >= closeTime)
        {
            return ServiceResult<FacilityDto>.Failure("Açılış saati kapanış saatinden önce olmalıdır.");
        }

        facility.Name = dto.Name.Trim();
        facility.Description = dto.Description?.Trim() ?? string.Empty;
        facility.HourlyPrice = dto.HourlyPrice;
        facility.OpenTime = openTime;
        facility.CloseTime = closeTime;
        facility.IsActive = dto.IsActive;
        facility.UpdatedAt = DateTime.UtcNow;

        await _facilityRepository.UpdateAsync(facility);

        var resultDto = _mapper.Map<FacilityDto>(facility);
        return ServiceResult<FacilityDto>.Success(resultDto);
    }

    public async Task<ServiceResult> DeleteAsync(Guid id)
    {
        var facility = await _facilityRepository.GetByIdAsync(id);
        if (facility == null)
        {
            return ServiceResult.Failure("Tesis bulunamadı.", 404);
        }

        // Soft delete — tesisi pasif yap
        facility.IsActive = false;
        facility.UpdatedAt = DateTime.UtcNow;
        await _facilityRepository.UpdateAsync(facility);

        return ServiceResult.Success();
    }
}
