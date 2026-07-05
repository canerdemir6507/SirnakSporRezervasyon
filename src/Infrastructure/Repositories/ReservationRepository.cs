using Microsoft.EntityFrameworkCore;
using SirnakSport.Domain.Entities;
using SirnakSport.Domain.Enums;
using SirnakSport.Domain.Interfaces;
using SirnakSport.Infrastructure.Data;

namespace SirnakSport.Infrastructure.Repositories;

/// <summary>
/// Rezervasyon repository implementasyonu.
/// Çakışma kontrol algoritması burada yer alır.
/// </summary>
public class ReservationRepository : Repository<Reservation>, IReservationRepository
{
    public ReservationRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Reservation>> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Include(r => r.User)
            .Include(r => r.Facility)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.Date)
            .ThenByDescending(r => r.StartTime)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetByFacilityAndDateAsync(Guid facilityId, DateOnly date)
    {
        return await _dbSet
            .Where(r => r.FacilityId == facilityId && r.Date == date)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// ⚠️ KRİTİK: Çakışma kontrol algoritması.
    /// Aynı tesis + aynı tarih + zaman çakışması + iptal edilmemiş rezervasyon kontrolü.
    /// 
    /// Çakışma mantığı:
    /// İki zaman aralığı (A ve B) çakışır eğer:
    ///   A.Start < B.End VE A.End > B.Start
    /// </summary>
    public async Task<bool> HasConflictAsync(
        Guid facilityId,
        DateOnly date,
        TimeSpan startTime,
        TimeSpan endTime,
        Guid? excludeReservationId = null)
    {
        var query = _dbSet
            .Where(r => r.FacilityId == facilityId
                && r.Date == date
                && r.Status != ReservationStatus.Cancelled
                && r.StartTime < endTime
                && r.EndTime > startTime);

        // Güncelleme senaryosunda mevcut rezervasyonu hariç tut
        if (excludeReservationId.HasValue)
        {
            query = query.Where(r => r.Id != excludeReservationId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<IEnumerable<Reservation>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(r => r.User)
            .Include(r => r.Facility)
            .OrderByDescending(r => r.Date)
            .ThenByDescending(r => r.StartTime)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Reservation?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(r => r.User)
            .Include(r => r.Facility)
            .FirstOrDefaultAsync(r => r.Id == id);
    }
}
