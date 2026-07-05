using SirnakSport.Domain.Entities;

namespace SirnakSport.Domain.Interfaces;

/// <summary>
/// Rezervasyon repository sözleşmesi.
/// </summary>
public interface IReservationRepository : IRepository<Reservation>
{
    Task<IEnumerable<Reservation>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Reservation>> GetByFacilityAndDateAsync(Guid facilityId, DateOnly date);
    Task<bool> HasConflictAsync(Guid facilityId, DateOnly date, TimeSpan startTime, TimeSpan endTime, Guid? excludeReservationId = null);
    Task<IEnumerable<Reservation>> GetAllWithDetailsAsync();
    Task<Reservation?> GetByIdWithDetailsAsync(Guid id);
}
