using Microsoft.EntityFrameworkCore;
using SirnakSport.Domain.Entities;
using SirnakSport.Domain.Interfaces;
using SirnakSport.Infrastructure.Data;

namespace SirnakSport.Infrastructure.Repositories;

/// <summary>
/// Tesis repository implementasyonu.
/// </summary>
public class FacilityRepository : Repository<Facility>, IFacilityRepository
{
    public FacilityRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Facility>> GetActiveFacilitiesAsync()
    {
        return await _dbSet
            .Where(f => f.IsActive)
            .AsNoTracking()
            .OrderBy(f => f.Name)
            .ToListAsync();
    }
}
