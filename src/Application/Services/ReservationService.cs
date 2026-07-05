using SirnakSport.Application.Common;
using SirnakSport.Application.DTOs.Reservation;
using SirnakSport.Application.Interfaces;
using SirnakSport.Domain.Entities;
using SirnakSport.Domain.Enums;
using SirnakSport.Domain.Interfaces;

namespace SirnakSport.Application.Services;

/// <summary>
/// Rezervasyon yönetimi servisi implementasyonu.
/// Çakışma kontrolü, fiyat hesaplama, müsaitlik sorgulama.
/// </summary>
public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IFacilityRepository _facilityRepository;
    private readonly IUserRepository _userRepository;

    public ReservationService(
        IReservationRepository reservationRepository,
        IFacilityRepository facilityRepository,
        IUserRepository userRepository)
    {
        _reservationRepository = reservationRepository;
        _facilityRepository = facilityRepository;
        _userRepository = userRepository;
    }

    public async Task<ServiceResult<ReservationDto>> CreateAsync(Guid userId, CreateReservationDto dto)
    {
        // Tarih parse
        if (!DateOnly.TryParse(dto.Date, out var date))
        {
            return ServiceResult<ReservationDto>.Failure("Geçersiz tarih formatı. yyyy-MM-dd formatında giriniz.");
        }

        // Geçmiş tarih kontrolü
        if (date < DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return ServiceResult<ReservationDto>.Failure("Geçmiş bir tarihe rezervasyon yapılamaz.");
        }

        // Saat parse
        if (!TimeSpan.TryParse(dto.StartTime, out var startTime))
        {
            return ServiceResult<ReservationDto>.Failure("Geçersiz başlangıç saati formatı. HH:mm formatında giriniz.");
        }

        if (!TimeSpan.TryParse(dto.EndTime, out var endTime))
        {
            return ServiceResult<ReservationDto>.Failure("Geçersiz bitiş saati formatı. HH:mm formatında giriniz.");
        }

        if (startTime >= endTime)
        {
            return ServiceResult<ReservationDto>.Failure("Başlangıç saati bitiş saatinden önce olmalıdır.");
        }

        // Tam saat kontrolü (hour-based booking)
        if (startTime.Minutes != 0 || endTime.Minutes != 0)
        {
            return ServiceResult<ReservationDto>.Failure("Rezervasyonlar yalnızca tam saat aralıklarında yapılabilir.");
        }

        // Tesis kontrolü
        var facility = await _facilityRepository.GetByIdAsync(dto.FacilityId);
        if (facility == null || !facility.IsActive)
        {
            return ServiceResult<ReservationDto>.Failure("Tesis bulunamadı veya aktif değil.", 404);
        }

        // Çalışma saatleri kontrolü
        if (startTime < facility.OpenTime || endTime > facility.CloseTime)
        {
            return ServiceResult<ReservationDto>.Failure(
                $"Rezervasyon, tesisin çalışma saatleri içinde olmalıdır. ({facility.OpenTime:hh\\:mm} - {facility.CloseTime:hh\\:mm})");
        }

        // Kullanıcı kontrolü
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return ServiceResult<ReservationDto>.Failure("Kullanıcı bulunamadı.", 404);
        }

        // ⚠️ ÇAKIŞMA KONTROLÜ — Kritik iş kuralı
        var hasConflict = await _reservationRepository.HasConflictAsync(
            dto.FacilityId, date, startTime, endTime);

        if (hasConflict)
        {
            return ServiceResult<ReservationDto>.Failure(
                "Bu zaman aralığında başka bir rezervasyon bulunmaktadır. Lütfen farklı bir saat seçiniz.", 409);
        }

        // Toplam fiyat hesapla (saat bazlı)
        var totalHours = (decimal)(endTime - startTime).TotalHours;
        var totalPrice = totalHours * facility.HourlyPrice;

        // Rezervasyon oluştur
        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FacilityId = dto.FacilityId,
            Date = date,
            StartTime = startTime,
            EndTime = endTime,
            Status = ReservationStatus.Pending,
            TotalPrice = totalPrice,
            CreatedAt = DateTime.UtcNow
        };

        await _reservationRepository.AddAsync(reservation);

        var resultDto = MapToDto(reservation, user, facility);
        return ServiceResult<ReservationDto>.Success(resultDto, 201);
    }

    public async Task<ServiceResult> CancelAsync(Guid reservationId, Guid userId, bool isAdmin)
    {
        var reservation = await _reservationRepository.GetByIdWithDetailsAsync(reservationId);
        if (reservation == null)
        {
            return ServiceResult.Failure("Rezervasyon bulunamadı.", 404);
        }

        // Yetki kontrolü — kullanıcı sadece kendi rezervasyonunu iptal edebilir
        if (!isAdmin && reservation.UserId != userId)
        {
            return ServiceResult.Failure("Bu rezervasyonu iptal etme yetkiniz yoktur.", 403);
        }

        if (reservation.Status == ReservationStatus.Cancelled)
        {
            return ServiceResult.Failure("Bu rezervasyon zaten iptal edilmiştir.");
        }

        reservation.Status = ReservationStatus.Cancelled;
        reservation.UpdatedAt = DateTime.UtcNow;
        await _reservationRepository.UpdateAsync(reservation);

        return ServiceResult.Success();
    }

    public async Task<ServiceResult<IEnumerable<ReservationDto>>> GetByUserAsync(Guid userId)
    {
        var reservations = await _reservationRepository.GetByUserIdAsync(userId);
        var dtos = reservations.Select(r => MapToDto(r, r.User, r.Facility));
        return ServiceResult<IEnumerable<ReservationDto>>.Success(dtos);
    }

    public async Task<ServiceResult<IEnumerable<ReservationDto>>> GetAllAsync()
    {
        var reservations = await _reservationRepository.GetAllWithDetailsAsync();
        var dtos = reservations.Select(r => MapToDto(r, r.User, r.Facility));
        return ServiceResult<IEnumerable<ReservationDto>>.Success(dtos);
    }

    public async Task<ServiceResult<IEnumerable<AvailableSlotDto>>> GetAvailableSlotsAsync(Guid facilityId, string dateStr)
    {
        if (!DateOnly.TryParse(dateStr, out var date))
        {
            return ServiceResult<IEnumerable<AvailableSlotDto>>.Failure("Geçersiz tarih formatı. yyyy-MM-dd formatında giriniz.");
        }

        var facility = await _facilityRepository.GetByIdAsync(facilityId);
        if (facility == null || !facility.IsActive)
        {
            return ServiceResult<IEnumerable<AvailableSlotDto>>.Failure("Tesis bulunamadı veya aktif değil.", 404);
        }

        // Mevcut rezervasyonları al (iptal edilmemiş)
        var existingReservations = await _reservationRepository.GetByFacilityAndDateAsync(facilityId, date);
        var activeReservations = existingReservations
            .Where(r => r.Status != ReservationStatus.Cancelled)
            .ToList();

        // Tüm saatlik slotları oluştur
        var slots = new List<AvailableSlotDto>();
        var currentTime = facility.OpenTime;

        while (currentTime < facility.CloseTime)
        {
            var slotEnd = currentTime.Add(TimeSpan.FromHours(1));
            if (slotEnd > facility.CloseTime) break;

            var isOccupied = activeReservations.Any(r =>
                r.StartTime < slotEnd && r.EndTime > currentTime);

            slots.Add(new AvailableSlotDto
            {
                StartTime = currentTime.ToString(@"hh\:mm"),
                EndTime = slotEnd.ToString(@"hh\:mm"),
                IsAvailable = !isOccupied
            });

            currentTime = slotEnd;
        }

        return ServiceResult<IEnumerable<AvailableSlotDto>>.Success(slots);
    }

    public async Task<ServiceResult> ApproveAsync(Guid reservationId)
    {
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
        if (reservation == null)
        {
            return ServiceResult.Failure("Rezervasyon bulunamadı.", 404);
        }

        if (reservation.Status == ReservationStatus.Cancelled)
        {
            return ServiceResult.Failure("İptal edilmiş bir rezervasyon onaylanamaz.");
        }

        if (reservation.Status == ReservationStatus.Approved)
        {
            return ServiceResult.Failure("Bu rezervasyon zaten onaylanmıştır.");
        }

        reservation.Status = ReservationStatus.Approved;
        reservation.UpdatedAt = DateTime.UtcNow;
        await _reservationRepository.UpdateAsync(reservation);

        return ServiceResult.Success();
    }

    /// <summary>
    /// Reservation entity'sini ReservationDto'ya dönüştürür.
    /// </summary>
    private static ReservationDto MapToDto(Reservation reservation, User user, Facility facility)
    {
        return new ReservationDto
        {
            Id = reservation.Id,
            UserId = reservation.UserId,
            UserFullName = user.FullName,
            UserStudentNumber = user.StudentNumber,
            FacilityId = reservation.FacilityId,
            FacilityName = facility.Name,
            Date = reservation.Date.ToString("yyyy-MM-dd"),
            StartTime = reservation.StartTime.ToString(@"hh\:mm"),
            EndTime = reservation.EndTime.ToString(@"hh\:mm"),
            Status = reservation.Status.ToString(),
            TotalPrice = reservation.TotalPrice,
            CreatedAt = reservation.CreatedAt
        };
    }
}
