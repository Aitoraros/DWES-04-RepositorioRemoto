using Refit;
using RepositorioRemoto.Dto;

namespace RepositorioRemoto.Api;

/// <summary>Cliente tipado de la API REST remota (JSONPlaceholder).</summary>
public interface IJsonPlaceholderApi
{
    [Get("/users")]
    Task<List<UserDto>> GetAllAsync();

    [Get("/users/{id}")]
    Task<UserDto> GetByIdAsync(int id);

    [Post("/users")]
    Task<UserDto> CreateAsync([Body] UserDto user);

    [Put("/users/{id}")]
    Task<UserDto> UpdateAsync(int id, [Body] UserDto user);

    [Delete("/users/{id}")]
    Task DeleteAsync(int id);
}