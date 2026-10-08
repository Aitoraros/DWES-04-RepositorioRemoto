using Microsoft.Extensions.Configuration;

namespace RepositorioRemoto.Config;

public class AppConfig
{
    private static IConfigurationRoot Configuration { get; }

    // encendido
    static AppConfig()
    {
        // el entorno se lee de la variable de sistema, no de Configuration (que aún no existe)
        var environment = System.Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";

        Configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{environment}.json", true, true) // opcional: puede no existir
            .AddEnvironmentVariables()
            .Build();
    }

    public static string BaseUrl =>
        Configuration.GetValue<string>("ApiSettings:BaseUrl") ?? "https://jsonplaceholder.typicode.com";

    public static string Environment =>
        Configuration.GetValue<string>("AppSettings:Environment") ?? "Development";

    public static bool IsDevelopment =>
        Environment.Equals("Development", StringComparison.OrdinalIgnoreCase);

    public static string SqliteConnectionString =>
        Configuration.GetConnectionString("Sqlite") ?? "Data Source=usuarios.db";

    public static string PostgresConnectionString =>
        Configuration.GetConnectionString("PostgreSQL")
        ?? throw new InvalidOperationException("Falta la cadena de conexión 'PostgreSQL' en appsettings.json");

    public static string RedisConnectionString =>
        Configuration.GetValue<string>("RedisSettings:ConnectionString") ?? "localhost:6379";

    public static bool RedisDropData =>
        Configuration.GetValue<bool>("RedisSettings:DropData");
}