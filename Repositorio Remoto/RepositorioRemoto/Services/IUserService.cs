using RepositorioRemoto.Entity;
using RepositorioRemoto.Models;

namespace RepositorioRemoto.Services;

public interface IUserService
{
    Task<IEnumerable<UserEntity>> GetAllAsync();
    Task<UserEntity?> GetByIdAsync(int id);
    Task<UserEntity> AddAsync(UserEntity user);
    Task<UserEntity?> UpdateAsync(UserEntity user);
    Task<bool> DeleteAsync(int id);
}