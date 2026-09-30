using CSharpFunctionalExtensions;
using RepositorioRemoto.Errors.Common;
using RepositorioRemoto.Errors.User;
using Serilog;

namespace RepositorioRemoto.Validators;

/// <summary>Valida los datos de una petición de creación/actualización de usuario.</summary>
public class CreateUserRequestValidator : IValidator<CreateUserRequest>
{
    /// <inheritdoc cref="IValidator{T}.Validar" />
    public Result<CreateUserRequest, DomainError> Validar(CreateUserRequest request)
    {
        Log.Debug("🔵 Validando usuario con alias: {Alias}", request.Alias);

        var errores = new List<string>();

        if (!request.Nombre.IsNombreValid())
            errores.Add("El nombre es obligatorio y debe tener entre 2 y 50 caracteres.");

        if (!request.Alias.IsAliasValid())
            errores.Add("El alias es obligatorio, debe tener entre 3 y 20 caracteres y no puede contener espacios.");

        if (!request.Email.IsEmailValid())
            errores.Add("El email es obligatorio y tiene que seguir el formato 'xxx@xxx.xxx'.");

        return errores.Count > 0
            ? Result.Failure<CreateUserRequest, DomainError>(new UserErrors.Validation(errores))
            : Result.Success<CreateUserRequest, DomainError>(request);
    }
}