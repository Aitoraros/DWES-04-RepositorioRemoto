using System.Net;
using CSharpFunctionalExtensions;
using Refit;
using RepositorioRemoto.Api;
using RepositorioRemoto.Cache.Common;
using RepositorioRemoto.Dto;
using RepositorioRemoto.Errors.Api;
using RepositorioRemoto.Errors.Common;
using RepositorioRemoto.Errors.Storage;
using RepositorioRemoto.Errors.User;
using RepositorioRemoto.Mappers;
using RepositorioRemoto.Models;
using RepositorioRemoto.Repositories.Common;
using RepositorioRemoto.Services.Notificactions;
using RepositorioRemoto.Storage;
using RepositorioRemoto.Validators;
using Serilog;

namespace RepositorioRemoto.Services.Users;

public class UserService(
    IValidator<CreateUserRequest> createValidator,
    IValidator<UpdateUserRequest> updateValidator,
    IUserRepository repository,
    ICache<int, User> cache,
    IJsonPlaceholderApi api, 
    INotificationService notificationService,
    IUserStorage storage) : IUserService
{
    private readonly ILogger _logger = Log.ForContext<UserService>();

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        var locales = await repository.GetAllAsync();
        var lista = locales.ToList();

        if (lista.Count > 0)
            return lista.Select(e => e.ToModel());

        try
        {
            var dtos = await api.GetAllAsync();
            var entidades = dtos.Select(dto => dto.ToModel().ToEntity()).ToList();
            await repository.InsertRangeAsync(entidades);
            return entidades.Select(e => e.ToModel());
        }
        catch (ApiException)
        {
            _logger.Warning("No se pudo recuperar la lista de usuarios de la API.");
            return [];
        }
    }

    public async Task<Result<User, DomainError>> GetByIdAsync(int id)
    {
        var cacheado = await cache.Get(id);
        if (cacheado is not null)
            return Result.Success<User, DomainError>(cacheado);

        var local = await repository.GetByIdAsync(id);
        if (local is not null)
        {
            var modelo = local.ToModel();
            await cache.Add(id, modelo);
            return Result.Success<User, DomainError>(modelo);
        }

        try
        {
            var dto = await api.GetByIdAsync(id);
            var usuario = dto.ToModel();
            await repository.CreateAsync(usuario.ToEntity());
            await cache.Add(id, usuario);
            return Result.Success<User, DomainError>(usuario);
        }
        catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Failure<User, DomainError>(UserErrors.NotFound(id));
        }
        catch (ApiException ex)
        {
            return Result.Failure<User, DomainError>(ApiErrors.BadResponse((int)ex.StatusCode, ex.Message));
        }
    }

    public async Task<Result<User, DomainError>> CreateAsync(CreateUserRequest request)
    {
        var validacion = createValidator.Validar(request);
        if (validacion.IsFailure)
            return Result.Failure<User, DomainError>(validacion.Error);

        try
        {
            var dto = request.ToModel().ToDto();
            var creado = await api.CreateAsync(dto);

            var usuario = request.ToModel(creado.Id);
            var guardado = await repository.CreateAsync(usuario.ToEntity());
            await cache.Add(guardado.Id, usuario);

            _logger.Information("Usuario creado con Id {Id}.", guardado.Id);
            
            notificationService.Notificar(new Notification(
                Notification.NotificationType.Create,
                $"Se ha creado el usuario con Id {guardado.Id}.",
                DateTime.UtcNow));
            
            return Result.Success<User, DomainError>(usuario);
        }
        catch (ApiException ex)
        {
            return Result.Failure<User, DomainError>(ApiErrors.BadResponse((int)ex.StatusCode, ex.Message));
        }
    }

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
            await api.UpdateAsync(id, dto);

            var usuario = request.ToModel();
            var actualizado = await repository.UpdateAsync(usuario.ToEntity());

            if (actualizado is null)
                return Result.Failure<User, DomainError>(UserErrors.NotFound(id));

            var modelo = actualizado.ToModel();
            await cache.Add(id, modelo);

            _logger.Information("Usuario {Id} actualizado.", id);
            
            notificationService.Notificar(new Notification(
                Notification.NotificationType.Update,
                $"Se ha actualizado el usuario con Id {id}.",
                DateTime.UtcNow));
            
            return Result.Success<User, DomainError>(modelo);
        }
        catch (ApiException ex)
        {
            return Result.Failure<User, DomainError>(ApiErrors.BadResponse((int)ex.StatusCode, ex.Message));
        }
    }

    public async Task<Result<User, DomainError>> DeleteAsync(int id)
    {
        var existe = await repository.GetByIdAsync(id);
        if (existe is null)
            return Result.Failure<User, DomainError>(UserErrors.NotFound(id));

        try
        {
            await api.DeleteAsync(id);

            await repository.DeleteAsync(id);
            await cache.Remove(id);

            _logger.Information("Usuario {Id} eliminado.", id);
            
            notificationService.Notificar(new Notification(
                Notification.NotificationType.Delete,
                $"Se ha eliminado el usuario con Id {id}.",
                DateTime.UtcNow));
            
            return Result.Success<User, DomainError>(existe.ToModel());
        }
        catch (ApiException ex)
        {
            return Result.Failure<User, DomainError>(ApiErrors.BadResponse((int)ex.StatusCode, ex.Message));
        }
    }

    public async Task<Result<string, DomainError>> ExportAsync()
    {
        try
        {
            var usuarios = (await GetAllAsync()).ToList();
            var ruta = await storage.ExportAsync(usuarios);

            _logger.Information("Usuarios exportados a {Ruta}.", ruta);

            return Result.Success<string, DomainError>(ruta);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "No se pudo exportar la lista de usuarios.");
            return Result.Failure<string, DomainError>(StorageErrors.WriteError(ex.Message));
        }
    }
}