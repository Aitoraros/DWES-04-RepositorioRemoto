using RepositorioRemoto.Errors.Common;

namespace RepositorioRemoto.Errors.User;

/// <summary>Factory para crear errores de Usuario</summary>
public static class UserErrors {
    
    /// <inheritdoc cref="UserError.NotFound"/>
    public static DomainError NotFound(int id) =>
        new UserError.NotFound(id);

    /// <inheritdoc cref="UserError.Validation"/>
    public static DomainError Validation(IReadOnlyList<string> errors) =>
        new UserError.Validation(errors);

    /// <inheritdoc cref="UserError.ExportFailure"/>
    public static DomainError ExportFailure(string detail) =>
        new UserError.ExportFailure(detail);
}