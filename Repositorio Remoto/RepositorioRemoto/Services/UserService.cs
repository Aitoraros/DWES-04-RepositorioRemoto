using CSharpFunctionalExtensions;
using RepositorioRemoto.Api;
using RepositorioRemoto.Cache.Common;
using RepositorioRemoto.Dto;
using RepositorioRemoto.Errors.Api;
using RepositorioRemoto.Errors.Common;
using RepositorioRemoto.Errors.User;
using RepositorioRemoto.Mappers;
using RepositorioRemoto.Models;
using RepositorioRemoto.Repositories.Common;
using RepositorioRemoto.Validators;
using Serilog;

namespace RepositorioRemoto.Services;

public class UserService(
    IValidator<CreateUserRequest> createValidator,
    IValidator<UpdateUserRequest> updateValidator,
    IUserRepository repository,
    ICache<int, User> cache,
    IJsonPlaceholderApi api) : IUserService
{
    private readonly ILogger _logger = Log.ForContext<UserService>();
    private static string CacheKey(int id) => $"User:{id}";

    /// <inheritdoc />
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        var locales = await repository.GetAllAsync();
        var lista = locales.ToList();

        if (lista.Count > 0)
            return lista.Select(e => e.ToModel());

        _logger.Information("BD vacía, recuperando usuarios desde la API...");
        var respuesta = await api.GetAllAsync();

        if (!respuesta.IsSuccessStatusCode || respuesta.Content is null)
        {
            _logger.Warning("No se pudo recuperar la lista de usuarios de la API.");
            return [];
        }

        var entidades = respuesta.Content.Select(dto => dto.ToModel().ToEntity()).ToList();
        await repository.InsertRangeAsync(entidades);

        return entidades.Select(e => e.ToModel());
    }

    /// <inheritdoc />
    public async Task<Result<User, DomainError>> GetByIdAsync(int id)
    {
        // 1. Caché
        var cacheado = await cache.Get(id);
        if (cacheado is not null)
            return Result.Success<User, DomainError>(cacheado);

        // 2. BD local
        var local = await repository.GetByIdAsync(id);
        if (local is not null)
        {
            var modelo = local.ToModel();
            await cache.Add(id, modelo);
            return Result.Success<User, DomainError>(modelo);
        }

        // 3. API remota
        try
        {
            var respuesta = await api.GetByIdAsync(id);

            if (!respuesta.IsSuccessStatusCode || respuesta.Content is null)
                return Result.Failure<User, DomainError>(UserErrors.NotFound(id));

            var usuario = respuesta.Content.ToModel();
            await repository.CreateAsync(usuario.ToEntity());
            await cache.Add(id, usuario);

            return Result.Success<User, DomainError>(usuario);
        }
        catch (HttpRequestException ex)
        {
            return Result.Failure<User, DomainError>(ApiErrors.Unavailable(ex.Message));
        }
    }

    /// <inheritdoc />
    public async Task<Result<User, DomainError>> CreateAsync(CreateUserRequest request)
    {
        var validacion = createValidator.Validar(request);
        if (validacion.IsFailure)
            return Result.Failure<User, DomainError>(validacion.Error);

        try
        {
            var dto = request.ToModel().ToDto();
            var respuesta = await api.CreateAsync(dto);

            if (!respuesta.IsSuccessStatusCode || respuesta.Content is null)
                return Result.Failure<User, DomainError>(
                    ApiErrors.BadResponse((int)respuesta.StatusCode!, respuesta.Error?.Message ?? "Error desconocido"));

            // el id definitivo lo asigna la API
            var usuario = request.ToModel(respuesta.Content.Id);

            var creado = await repository.CreateAsync(usuario.ToEntity());
            await cache.Add(creado.Id, usuario);

            _logger.Information("Usuario creado con Id {Id}.", creado.Id);
            return Result.Success<User, DomainError>(usuario);
        }
        catch (HttpRequestException ex)
        {
            return Result.Failure<User, DomainError>(ApiErrors.Unavailable(ex.Message));
        }
    }

    /// <inheritdoc />
    public async Task<Result<User, DomainError>> UpdateAsync(int id, UpdateUserRequest request)
    {
        if (id != request.Id)
            return Result.Failure<User, DomainError>(UserErrors.Validation(["El id de la ruta no coincide con el id del cuerpo."]));

        var validacion = updateValidator.Validar(request);
        if (validacion.IsFailure)
            return Result.Failure<User, DomainError>(validacion.Error);

        var existe = await repository.GetByIdAsync(id);
        if (existe is null)
            return Result.Failure<User, DomainError>(UserErrors.NotFound(id));

        try
        {
            var dto = request.ToModel().ToDto();
            var respuesta = await api.UpdateAsync(id, dto);

            if (!respuesta.IsSuccessStatusCode)
                return Result.Failure<User, DomainError>(
                    ApiErrors.BadResponse((int)respuesta.StatusCode!, respuesta.Error?.Message ?? "Error desconocido"));

            var usuario = request.ToModel();
            var actualizado = await repository.UpdateAsync(usuario.ToEntity());

            if (actualizado is null)
                return Result.Failure<User, DomainError>(UserErrors.NotFound(id));

            var modelo = actualizado.ToModel();
            await cache.Add(id, modelo);

            _logger.Information("Usuario {Id} actualizado.", id);
            return Result.Success<User, DomainError>(modelo);
        }
        catch (HttpRequestException ex)
        {
            return Result.Failure<User, DomainError>(ApiErrors.Unavailable(ex.Message));
        }
    }

    /// <inheritdoc />
    public async Task<Result<User, DomainError>> DeleteAsync(int id)
    {
        var existe = await repository.GetByIdAsync(id);
        if (existe is null)
            return Result.Failure<User, DomainError>(UserErrors.NotFound(id));

        try
        {
            var respuesta = await api.DeleteAsync(id);

            if (!respuesta.IsSuccessStatusCode)
                return Result.Failure<User, DomainError>(
                    ApiErrors.BadResponse((int)respuesta.StatusCode!, "No se pudo eliminar en la API."));

            await repository.DeleteAsync(id);
            await cache.Remove(id);

            _logger.Information("Usuario {Id} eliminado.", id);
            return Result.Success<User, DomainError>(existe.ToModel());
        }
        catch (HttpRequestException ex)
        {
            return Result.Failure<User, DomainError>(ApiErrors.Unavailable(ex.Message));
        }
    }
}