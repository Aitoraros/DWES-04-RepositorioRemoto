using Refit;
using RepositorioRemoto.Dto;

namespace RepositorioRemoto.Api;

/// <summary>Cliente tipado de la API REST remota (JSONPlaceholder).</summary>
public interface IJsonPlaceholderApi
{
    [Get("/users")]
    Task<ApiResponse<List<UserDto>>> GetAllAsync();

    [Get("/users/{id}")]
    Task<ApiResponse<UserDto>> GetByIdAsync(int id);

    [Post("/users")]
    Task<ApiResponse<UserDto>> CreateAsync([Body] UserDto user);

    [Put("/users/{id}")]
    Task<ApiResponse<UserDto>> UpdateAsync(int id, [Body] UserDto user);

    [Delete("/users/{id}")]
    Task<IApiResponse> DeleteAsync(int id);
}