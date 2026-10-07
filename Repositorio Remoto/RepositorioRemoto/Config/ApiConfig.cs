
namespace RepositorioRemoto.Config;

/// <summary>Configuración de la aplicación: API remota, sincronización y exportación.</summary>
public class ApiConfig
{
    public const string Section = "ApiSettings";

    public string BaseUrl { get; set; } = "https://jsonplaceholder.typicode.com";
    public int TimeoutSeconds { get; set; } = 10;
    public int SyncIntervalSeconds { get; set; } = 60;
    public string ExportDirectory { get; set; } = "exports";

    /// <summary>Proveedor de base de datos: "Sqlite" o "Postgres".</summary>
    public string DatabaseProvider { get; set; } = "Sqlite";

    /// <summary>Cadena de conexión de SQLite.</summary>
    public string SqliteConnectionString { get; set; } = "Data Source=usuarios.db";

}