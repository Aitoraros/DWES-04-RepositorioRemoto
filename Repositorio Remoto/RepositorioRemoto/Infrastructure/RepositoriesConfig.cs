using Microsoft.Extensions.DependencyInjection;
using RepositorioRemoto.Repositories;
using RepositorioRemoto.Repositories.Common;
using RepositorioRemoto.Repositories.Sqlite;

namespace RepositorioRemoto.Infrastructure;

public static class RepositoriesConfig
{
    /// <summary>
    /// Registra los repositorios en el contenedor de dependencias.
    /// </summary>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Singleton porque el repositorio usa Dictionary en memoria.
        // Si fuera Scoped, cada request perdería los datos.
        // En producción con BD real, sería AddScoped.
        services.AddSingleton<IUserRepository, UserEfcoreRepository>();
        return services;
    }
}