using Microsoft.Extensions.Caching.Memory;
using RepositorioRemoto.Cache.Common;
using Serilog;

namespace RepositorioRemoto.Cache.Memory;

/// <summary>
/// Cache que implementa ICache usando MemoryCache
/// </summary>
public class MemoryCache<TKey, TValue>(IMemoryCache cache, TimeSpan? expiration = null) : ICache<TKey, TValue> where TKey : notnull {

    private readonly HashSet<TKey> _keys = [];
    private readonly ILogger _logger = Log.ForContext<MemoryCache<TKey, TValue>>();

    public Task Add(TKey key, TValue value) {
        
        var options = new MemoryCacheEntryOptions();
        if (expiration.HasValue)
            options.AbsoluteExpirationRelativeToNow = expiration.Value;

        cache.Set(key, value, options);
        _keys.Add(key);

        _logger.Debug("Clave {Key} guardada en MemoryCache.", key);
        return Task.CompletedTask;
    }

    public Task<TValue?> Get(TKey key) {
        
        var encontrado = cache.TryGetValue(key, out TValue? value);
        _logger.Debug("Clave {Key} {Resultado} en MemoryCache.", key, encontrado ? "encontrada" : "no encontrada");
        return Task.FromResult(value);
    }

    public Task<bool> Remove(TKey key) {
        
        cache.Remove(key);
        var eliminado = _keys.Remove(key);
        _logger.Debug("Clave {Key} eliminada de MemoryCache.", key);
        return Task.FromResult(eliminado);
    }

    public Task Clear() {
        
        foreach (var key in _keys)
            cache.Remove(key);

        _keys.Clear();
        _logger.Debug("MemoryCache borrada por completo.");
        return Task.CompletedTask;
    }

    public Task DisplayStatus() {
        _logger.Information("MemoryCache: {Count} claves almacenadas.", _keys.Count);
        return Task.CompletedTask;
    }
}