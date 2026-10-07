using Microsoft.Extensions.DependencyInjection;
using RepositorioRemoto.Services;

namespace RepositorioRemoto.Infrastructure;

public static class ServicesConfig
{
    /// <summary>
    /// Registra los servicios en el contenedor de dependencias.
    /// </summary>
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        return services;
    }
}