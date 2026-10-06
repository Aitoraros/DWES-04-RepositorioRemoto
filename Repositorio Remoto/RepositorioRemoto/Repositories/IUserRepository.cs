using RepositorioRemoto.Models;

namespace RepositorioRemoto.Repositories;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<User> AddAsync(User user);
    Task<User?> UpdateAsync(int id, User user);
    Task<bool> DeleteAsync(int id);
    Task ReplaceAllAsync(IEnumerable<User> users);   // borra todo e inserta (arranque y sincronización)
}