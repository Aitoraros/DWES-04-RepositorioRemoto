using System.Text.Json;
using RepositorioRemoto.Cache.Common;
using Serilog;
using StackExchange.Redis;

namespace RepositorioRemoto.Cache.Redis;

/// <summary>
/// Cache que implementa ICache udando Redis
/// </summary>
/// <param name="redis">Conexion</param>
public class RedisCache<TKey, TValue>(IConnectionMultiplexer redis, string prefix, TimeSpan? expiration = null) : ICache<TKey, TValue> where TKey : notnull {
    
    private readonly IDatabase _db = redis.GetDatabase();
    private readonly ILogger _logger = Log.ForContext<RedisCache<TKey, TValue>>();

    public async Task Add(TKey key, TValue value) {
        
        var redisKey = BuildKey(key);
        _logger.Debug("Guardando clave {Key} en Redis...", redisKey);

        var json = JsonSerializer.Serialize(value);
        if (expiration.HasValue) {
            await _db.StringSetAsync(redisKey, json, expiration.Value);
        } else {
            await _db.StringSetAsync(redisKey, json);
        }

        _logger.Debug("Clave {Key} guardada con éxito.", redisKey);
    }

    public async Task<TValue?> Get(TKey key) {
        
        var redisKey = BuildKey(key);
        _logger.Debug("Buscando clave {Key} en Redis...", redisKey);

        var value = await _db.StringGetAsync(redisKey);
        if (value.IsNullOrEmpty) {
            _logger.Debug("Clave {Key} no encontrada en Redis.", redisKey);
            return default;
        }

        _logger.Debug("Clave {Key} encontrada en Redis.", redisKey);
        return JsonSerializer.Deserialize<TValue>((string)value!);
    }

    public async Task<bool> Remove(TKey key) {
        
        var redisKey = BuildKey(key);
        _logger.Debug("Intentando eliminar clave {Key} de Redis...", redisKey);

        var eliminado = await _db.KeyDeleteAsync(redisKey);
        _logger.Debug("Clave {Key} eliminada: {Eliminado}", redisKey, eliminado);
        return eliminado;
    }

    public async Task Clear() {
        
        // Solo se borran las claves de ESTA caché (por prefijo), nunca toda la BD de Redis
        var server = redis.GetServer(redis.GetEndPoints().First());
        var keys = server.Keys(pattern: $"{prefix}:*").ToArray();

        if (keys.Length > 0)
            await _db.KeyDeleteAsync(keys);

        _logger.Debug("Caché Redis (prefijo '{Prefix}') borrada: {Count} claves eliminadas.", prefix, keys.Length);
    }

    public Task DisplayStatus() {
        
        var server = redis.GetServer(redis.GetEndPoints().First());
        var count = server.Keys(pattern: $"{prefix}:*").Count();

        _logger.Information("Redis ('{Prefix}'): {Count} claves almacenadas.", prefix, count);
        return Task.CompletedTask;
    }

    /// <summary>Construye la clave real de Redis añadiendo el preijo de esta cache</summary>
    private string BuildKey(TKey key) => $"{prefix}:{key}";
}