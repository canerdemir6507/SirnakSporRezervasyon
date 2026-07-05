using Microsoft.EntityFrameworkCore;
using SirnakSport.Domain.Entities;
using SirnakSport.Domain.Interfaces;
using SirnakSport.Infrastructure.Data;

namespace SirnakSport.Infrastructure.Repositories;

/// <summary>
/// Onaylı öğrenci repository implementasyonu.
/// </summary>
public class ApprovedStudentRepository : Repository<ApprovedStudent>, IApprovedStudentRepository
{
    public ApprovedStudentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<bool> IsStudentApprovedAsync(string studentNumber)
    {
        return await _dbSet.AnyAsync(a =>
            a.StudentNumber == studentNumber && a.IsActive);
    }

    public async Task<ApprovedStudent?> GetByStudentNumberAsync(string studentNumber)
    {
        return await _dbSet.FirstOrDefaultAsync(a =>
            a.StudentNumber == studentNumber);
    }

    public async Task<IEnumerable<ApprovedStudent>> GetActiveStudentsAsync()
    {
        return await _dbSet
            .Where(a => a.IsActive)
            .OrderBy(a => a.StudentNumber)
            .AsNoTracking()
            .ToListAsync();
    }
}
