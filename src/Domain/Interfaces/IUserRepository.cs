using SirnakSport.Domain.Entities;

namespace SirnakSport.Domain.Interfaces;

/// <summary>
/// Kullanıcıya özgü repository sözleşmesi.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByStudentNumberAsync(string studentNumber);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> StudentNumberExistsAsync(string studentNumber);
}
