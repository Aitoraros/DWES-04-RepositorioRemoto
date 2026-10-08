using RepositorioRemoto.Errors.Common;

namespace RepositorioRemoto.Errors.Storage;

/// <summary>Errores del almacenamiento de datos en ficheros.</summary>
public abstract record StorageError(string Message) : DomainError(Message)
{
    /// <summary>No se pudo escribir en el fichero o en la carpeta de destino.</summary>
    /// <param name="Detalle">Mensaje de la excepción original.</param>
    public sealed record WriteError(string Detalle)
        : StorageError($"Error al escribir en el almacenamiento: {Detalle}");
}

/// <summary>
/// Fábrica de errores de almacenamiento. Devuelve el tipo base <see cref="DomainError"/>
/// para poder usarlos directamente en <c>Result.Failure</c>.
/// </summary>
public static class StorageErrors
{
    /// <summary>Error al escribir en el almacenamiento.</summary>
    /// <param name="detalle">Mensaje de la excepción original.</param>
    public static DomainError WriteError(string detalle) => new StorageError.WriteError(detalle);
}