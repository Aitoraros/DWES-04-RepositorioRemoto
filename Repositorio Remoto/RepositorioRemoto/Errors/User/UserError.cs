using RepositorioRemoto.Errors.Common;

namespace RepositorioRemoto.Errors.User;

/// <summary>
/// Errores específicos del dominio de Usuario.
/// </summary>
public abstract record UserError(string Message) : DomainError(Message)
{
    /// <summary>Usuario no encontrado por ID (HTTP 404).</summary>
    public sealed record NotFoundById(int Id)
        : UserError($"No se ha encontrado ningún usuario con el identificador: {Id}");

    /// <summary>Datos de usuario no válidos (HTTP 400).</summary>
    public sealed record Validation(IEnumerable<string> Errores)
        : UserError($"Errores de validación:{Environment.NewLine}• {string.Join($"{Environment.NewLine}• ", Errores)}");

    /// <summary>Fallo de comunicación con la API externa para operaciones de usuario.</summary>
    public sealed record ApiFailure(int StatusCode, string Detail)
        : UserError($"Error de la API ({StatusCode}): {Detail}");
}