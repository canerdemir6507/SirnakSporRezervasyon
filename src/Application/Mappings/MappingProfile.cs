using AutoMapper;
using SirnakSport.Application.DTOs.ApprovedStudent;
using SirnakSport.Application.DTOs.Facility;
using SirnakSport.Application.DTOs.User;
using SirnakSport.Domain.Entities;

namespace SirnakSport.Application.Mappings;

/// <summary>
/// AutoMapper profil tanımları — Entity ↔ DTO dönüşümleri.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Facility → FacilityDto
        CreateMap<Facility, FacilityDto>()
            .ForMember(dest => dest.OpenTime, opt => opt.MapFrom(src => src.OpenTime.ToString(@"hh\:mm")))
            .ForMember(dest => dest.CloseTime, opt => opt.MapFrom(src => src.CloseTime.ToString(@"hh\:mm")));

        // User → UserDto
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

        // ApprovedStudent → ApprovedStudentDto
        CreateMap<ApprovedStudent, ApprovedStudentDto>();
    }
}
