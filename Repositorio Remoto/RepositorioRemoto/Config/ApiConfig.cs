
namespace RepositorioRemoto.Config;

/// <summary>Configuración de la aplicación: API remota, sincronización y exportación.</summary>
public class ApiConfig
{
    /// <summary>Nombre de la sección en appsettings.json.</summary>
    public const string Section = "ApiSettings";

    /// <summary>URL base de la API remota.</summary>
    public static string BaseUrl { get; set; } = "https://jsonplaceholder.typicode.com";

    /// <summary>Tiempo máximo de espera de cada petición a la API, en segundos.</summary>
    public int TimeoutSeconds { get; set; } = 10;

    /// <summary>Segundos entre una sincronización con la API y la siguiente.</summary>
    public int SyncIntervalSeconds { get; set; } = 60;

    /// <summary>Carpeta donde se guardan los ficheros JSON de la exportación.</summary>
    public string ExportDirectory { get; set; } = "exports";
}