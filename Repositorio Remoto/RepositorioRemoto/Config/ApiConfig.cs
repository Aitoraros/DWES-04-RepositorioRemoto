using Microsoft.Extensions.Configuration;

namespace RepositorioRemoto.Config;

public class ApiConfig
{
    private static IConfigurationRoot Configuration { get; }
    
    // encendido
    static ApiConfig() {
        Configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", false, true) // archivo no opcional y cambios reactivos
            .Build();
    }

    public static string BaseUrl => 
        Configuration.GetValue<string>("ApiSettings:BaseUrl") ?? "https://jsonplaceholder.typicode.com";
}