using CSharpFunctionalExtensions;
using RepositorioRemoto.Dto;
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

        // obligatorios
        if (!request.Nombre.IsNombreValid())
            errores.Add("El nombre es obligatorio y debe tener entre 2 y 50 caracteres.");

        if (!request.Alias.IsAliasValid())
            errores.Add("El alias es obligatorio, debe tener entre 3 y 20 caracteres y no puede contener espacios.");

        if (!request.Email.IsEmailValid())
            errores.Add("El email es obligatorio y tiene que seguir el formato 'xxx@xxx.xxx'.");

        // dirección
        if (!request.DireccionCalle.HasMaxLength(100))
            errores.Add("La calle no puede superar los 100 caracteres.");

        if (!request.DireccionSuite.HasMaxLength(50))
            errores.Add("La suite no puede superar los 50 caracteres.");

        if (!request.DireccionCiudad.HasMaxLength(50))
            errores.Add("La ciudad no puede superar los 50 caracteres.");

        if (!request.DireccionCodigoPostal.IsCodigoPostalValid())
            errores.Add("El código postal debe tener el formato '12345' o '12345-6789'.");

        if (!request.DireccionLatitud.IsLatitudValid())
            errores.Add("La latitud debe estar entre -90 y 90.");

        if (!request.DireccionLongitud.IsLongitudValid())
            errores.Add("La longitud debe estar entre -180 y 180.");

        // contacto
        if (!request.Telefono.IsTelefonoValid())
            errores.Add("El teléfono solo puede contener números, espacios y los símbolos + - ( ) . x (máximo 25 caracteres).");

        if (!request.Web.IsWebValid())
            errores.Add("La web debe tener un formato válido, por ejemplo 'midominio.com'.");

        // compañía
        if (!request.CompaniaNombre.HasMaxLength(100))
            errores.Add("El nombre de la compañía no puede superar los 100 caracteres.");

        if (!request.CompaniaEslogan.HasMaxLength(150))
            errores.Add("El eslogan no puede superar los 150 caracteres.");

        if (!request.CompaniaBs.HasMaxLength(150))
            errores.Add("El campo 'bs' de la compañía no puede superar los 150 caracteres.");

        return errores.Count > 0
            ? Result.Failure<CreateUserRequest, DomainError>(new UserError.Validation(errores))
            : Result.Success<CreateUserRequest, DomainError>(request);
    }
}