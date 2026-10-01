using RepositorioRemoto.Errors.Common;

namespace RepositorioRemoto.Errors.Api;

/// <summary>Errores de comunicación con la API remota (JSONPlaceholder).</summary>
public static class ApiErrors
{
    /// <summary>La API respondió con un código de error (502).</summary>
    /// <param name="StatusCode">Código HTTP devuelto por la API remota.</param>
    /// <param name="Detail">Detalle del error.</param>
    public sealed record BadResponse(int StatusCode, string Detail)
        : DomainError($"La API respondió con error ({StatusCode}): {Detail}");

    /// <summary>No se pudo contactar con la API: sin red, timeout... (503).</summary>
    /// <param name="Detail">Detalle técnico del fallo.</param>
    public sealed record Unavailable(string Detail)
        : DomainError($"API no disponible: {Detail}");
}