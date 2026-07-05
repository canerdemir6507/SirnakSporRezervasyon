using Microsoft.EntityFrameworkCore;
using SirnakSport.Domain.Entities;
using SirnakSport.Domain.Interfaces;
using SirnakSport.Infrastructure.Data;

namespace SirnakSport.Infrastructure.Repositories;

/// <summary>
/// Kullanıcı repository implementasyonu.
/// </summary>
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByStudentNumberAsync(string studentNumber)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.StudentNumber == studentNumber);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email);
    }

    public async Task<bool> StudentNumberExistsAsync(string studentNumber)
    {
        return await _dbSet.AnyAsync(u => u.StudentNumber == studentNumber);
    }
}
