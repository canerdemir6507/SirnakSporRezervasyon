using SirnakSport.Domain.Entities;

namespace SirnakSport.Domain.Interfaces;

/// <summary>
/// Tesis repository sözleşmesi.
/// </summary>
public interface IFacilityRepository : IRepository<Facility>
{
    Task<IEnumerable<Facility>> GetActiveFacilitiesAsync();
}
