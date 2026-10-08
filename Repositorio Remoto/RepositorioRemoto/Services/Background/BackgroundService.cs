using Microsoft.Extensions.DependencyInjection;
using RepositorioRemoto.Api;
using RepositorioRemoto.Cache.Common;
using RepositorioRemoto.Mappers;
using RepositorioRemoto.Models;
using RepositorioRemoto.Repositories.Common;
using Serilog;

namespace RepositorioRemoto.Services.Background;

public class BackgroundService(IServiceProvider provider)
{
    private readonly ILogger _logger = Log.ForContext<BackgroundService>();

    /// <summary>
    /// Inicia el bucle de sincronización periódica cada 60 segundos.
    /// </summary>
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(60));

        while (!cancellationToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken))
        {
            try
            {
                _logger.Information("Iniciando ciclo de sincronización de usuarios...");

                using var scope = provider.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var cache = scope.ServiceProvider.GetRequiredService<ICache<int, User>>();
                var api = scope.ServiceProvider.GetRequiredService<IJsonPlaceholderApi>();

                await Synchronize(repository, cache, api);

                _logger.Information("Sincronización completada con éxito.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error durante el ciclo de sincronización de usuarios.");
            }
        }
    }

    // limpieza
    private async Task Synchronize(IUserRepository repository, ICache<int, User> cache, IJsonPlaceholderApi api)
    {
        await cache.Clear();
        await repository.DeleteAllAsync();

        var dtos = await api.GetAllAsync();
        var entidades = dtos.Select(dto => dto.ToModel().ToEntity()).ToList();

        await repository.InsertRangeAsync(entidades);
    }
}