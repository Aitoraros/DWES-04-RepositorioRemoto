using RepositorioRemoto.Errors.Common;

namespace RepositorioRemoto.Errors.Api;

/// <summary>Factory para crear errores de la API remota</summary>
public static class ApiErrors {
    /// <inheritdoc cref="ApiError.BadResponse"/>
    public static DomainError BadResponse(int statusCode, string detail) =>
        new ApiError.BadResponse(statusCode, detail);

    /// <inheritdoc cref="ApiError.Unavailable"/>
    public static DomainError Unavailable(string detail) =>
        new ApiError.Unavailable(detail);
}