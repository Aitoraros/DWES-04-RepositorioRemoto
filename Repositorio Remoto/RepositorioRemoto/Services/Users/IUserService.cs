using CSharpFunctionalExtensions;
using RepositorioRemoto.Dto;
using RepositorioRemoto.Errors.Common;
using RepositorioRemoto.Models;

namespace RepositorioRemoto.Services.Users;

public interface IUserService {
    Task<IEnumerable<User>> GetAllAsync();
    Task<Result<User, DomainError>> GetByIdAsync(int id);
    Task<Result<User, DomainError>> CreateAsync(CreateUserRequest request);
    Task<Result<User, DomainError>> UpdateAsync(int id, UpdateUserRequest request);
    Task<Result<User, DomainError>> DeleteAsync(int id);
}