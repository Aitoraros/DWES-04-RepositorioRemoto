using RepositorioRemoto.Entity;
using RepositorioRemoto.Models;
using RepositorioRemoto.Repositories;

namespace RepositorioRemoto.Services;

public class UserService(IUserRepository repository) : IUserService
{
    public Task<IEnumerable<UserEntity>> GetAllAsync() => repository.GetAllAsync();

    public Task<UserEntity?> GetByIdAsync(int id) => repository.GetByIdAsync(id);

    public Task<UserEntity> AddAsync(UserEntity user) => repository.CreateAsync(user);

    public Task<UserEntity?> UpdateAsync(UserEntity user) => repository.UpdateAsync(user);

    public Task<bool> DeleteAsync(int id) => repository.DeleteAsync(id);
}