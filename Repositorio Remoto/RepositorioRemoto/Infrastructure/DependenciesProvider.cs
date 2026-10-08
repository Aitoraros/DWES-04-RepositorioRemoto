using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using RepositorioRemoto.Api;
using RepositorioRemoto.Cache.Common;
using RepositorioRemoto.Cache.Memory;
using RepositorioRemoto.Cache.Redis;
using RepositorioRemoto.Config;
using RepositorioRemoto.Dto;
using RepositorioRemoto.Entity;
using RepositorioRemoto.Models;
using RepositorioRemoto.Repositories.Common;
using RepositorioRemoto.Repositories.Dapper;
using RepositorioRemoto.Repositories.Sqlite;
using RepositorioRemoto.Services.Background;
using RepositorioRemoto.Services.Notificactions;
using RepositorioRemoto.Services.Notifications;
using RepositorioRemoto.Services.Users;
using RepositorioRemoto.Storage;
using RepositorioRemoto.Validators;
using StackExchange.Redis;

namespace RepositorioRemoto.Infrastructure;

/// <summary>
/// Config. de ID manual
/// </summary>
public static class DependenciesProvider
{
    public static IServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();

        // ---------- api (Refit) ----------
        services.AddRefitClient<IJsonPlaceholderApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(AppConfig.BaseUrl));

        // ---------- repositorio + caché: según entorno ----------
        if (AppConfig.IsDevelopment)
        {
            // Development: SQLite + MemoryCache (sin infraestructura externa)
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(AppConfig.SqliteConnectionString));

            // crear la BD y las tablas si no existen
            using (var context = new AppDbContext(
                new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlite(AppConfig.SqliteConnectionString)
                    .Options))
            {
                context.Database.EnsureCreated();
            }

            services.AddScoped<IUserRepository, UserEfcoreRepository>();

            services.AddMemoryCache();
            services.AddSingleton<ICache<int, User>>(sp =>
                new MemoryCache<int, User>(sp.GetRequiredService<IMemoryCache>()));
        }
        else
        {
            // Production: PostgreSQL + Redis
            using (var context = new AppDbContext(
                new DbContextOptionsBuilder<AppDbContext>()
                    .UseNpgsql(AppConfig.PostgresConnectionString)
                    .Options))
            {
                context.Database.EnsureCreated();
            }

            services.AddSingleton<IUserRepository>(_ =>
                new UserDapperRepository(AppConfig.PostgresConnectionString));

            services.AddSingleton<IConnectionMultiplexer>(_ =>
            {
                var multiplexer = ConnectionMultiplexer.Connect(AppConfig.RedisConnectionString);

                if (AppConfig.RedisDropData)
                {
                    var server = multiplexer.GetServer(multiplexer.GetEndPoints().First());
                    server.FlushDatabase();
                }

                return multiplexer;
            });

            services.AddSingleton<ICache<int, User>>(sp =>
                new RedisCache<int, User>(sp.GetRequiredService<IConnectionMultiplexer>(), prefix: "user"));
        }

        // ---------- almacenamiento / exportación ----------
        services.AddSingleton<IUserStorage>(_ => new UserStorage(AppConfig.ExportDirectory));

        // ---------- validadores ----------
        services.AddScoped<IValidator<CreateUserRequest>, CreateUserRequestValidator>();
        services.AddScoped<IValidator<UpdateUserRequest>, UpdateUserRequestValidator>();

        // ---------- notificaciones (Rx.NET) ----------
        services.AddSingleton<INotificationService, NotificationService>();

        // ---------- servicio principal ----------
        services.AddScoped<IUserService, UserService>();

        // ---------- sincronización en segundo plano ----------
        services.AddSingleton<BackgroundService>();

        return services.BuildServiceProvider();
    }
}