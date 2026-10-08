using RepositorioRemoto.Errors.Common;

namespace RepositorioRemoto.Errors.User;

/// <summary>Errores relacionados con los datos y operaciones de usuario.</summary>
public static class UserError
{
    /// <summary>Usuario no encontrado por ID (404).</summary>
    /// <param name="Id">Identificador buscado.</param>
    public sealed record NotFound(int Id)
        : DomainError($"No se ha encontrado ningún usuario con el identificador: {Id}");

    /// <summary>Datos de usuario no válidos (400).</summary>
    /// <param name="Errors">Lista de mensajes de validación.</param>
    public sealed record Validation(IReadOnlyList<string> Errors)
        : DomainError($"Errores de validación:{Environment.NewLine}• {string.Join($"{Environment.NewLine}• ", Errors)}");

    /// <summary>Fallo al escribir el fichero de exportación (500).</summary>
    /// <param name="Detail">Detalle técnico del fallo.</param>
    public sealed record ExportFailure(string Detail)
        : DomainError($"Error al exportar usuarios: {Detail}");
}