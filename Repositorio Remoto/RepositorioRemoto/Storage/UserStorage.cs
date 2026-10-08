using System.Text.Json;
using RepositorioRemoto.Models;

namespace RepositorioRemoto.Storage;

/// <summary>Exporta usuarios a ficheros JSON dentro de una carpeta.</summary>
/// <param name="directorio">Carpeta donde se guardan los ficheros (se crea si no existe).</param>
public class UserStorage(string directorio) : IUserStorage
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    /// <inheritdoc />
    public async Task<string> ExportAsync(IEnumerable<User> users)
    {
        ArgumentNullException.ThrowIfNull(users);
        
        var carpeta = Path.GetFullPath(directorio);
        Directory.CreateDirectory(carpeta);

        var ruta = Path.Combine(carpeta, $"users_{DateTime.Now:yyyyMMdd_HHmmss}.json");

        await using var stream = File.Create(ruta);
        await JsonSerializer.SerializeAsync(stream, users, JsonOptions);

        return ruta;
    }
}