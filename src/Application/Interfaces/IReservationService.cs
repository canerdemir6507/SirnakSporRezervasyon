using SirnakSport.Application.Common;
using SirnakSport.Application.DTOs.Reservation;

namespace SirnakSport.Application.Interfaces;

/// <summary>
/// Rezervasyon yönetimi servisi sözleşmesi.
/// </summary>
public interface IReservationService
{
    Task<ServiceResult<ReservationDto>> CreateAsync(Guid userId, CreateReservationDto dto);
    Task<ServiceResult> CancelAsync(Guid reservationId, Guid userId, bool isAdmin);
    Task<ServiceResult<IEnumerable<ReservationDto>>> GetByUserAsync(Guid userId);
    Task<ServiceResult<IEnumerable<ReservationDto>>> GetAllAsync();
    Task<ServiceResult<IEnumerable<AvailableSlotDto>>> GetAvailableSlotsAsync(Guid facilityId, string date);
    Task<ServiceResult> ApproveAsync(Guid reservationId);
}
