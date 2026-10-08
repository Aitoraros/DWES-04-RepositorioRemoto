using RepositorioRemoto.Models;

namespace RepositorioRemoto.Storage;

/// <summary>Almacenamiento de usuarios en ficheros externos.</summary>
public interface IUserStorage
{
    /// <summary>Exporta los usuarios a un fichero y devuelve su ruta.</summary>
    /// <param name="users">Usuarios a exportar.</param>
    /// <returns>Ruta completa del fichero generado.</returns>
    Task<string> ExportAsync(IEnumerable<User> users);
}