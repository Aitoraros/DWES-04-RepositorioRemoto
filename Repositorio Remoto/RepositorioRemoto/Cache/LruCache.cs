using RepositorioRemoto.Cache.Common;
using Serilog;

namespace RepositorioRemoto.Cache;

public class LruCache<TKey, TValue> : ICache<TKey, TValue> where TKey : notnull {
    
    private readonly int _capacidad;
    private readonly Dictionary<TKey, TValue> _data = new();
    private readonly LinkedList<TKey> _usageOrder = [];
    private readonly ILogger _logger = Log.ForContext<LruCache<TKey, TValue>>();

    public LruCache(int capacity) {
        if (capacity <= 0) 
            throw new ArgumentException("La capacidad de la caché debe ser mayor que 0.", nameof(capacity));
        _capacidad = capacity;
    }

    public Task Add(TKey key, TValue value) {
        
        _logger.Debug("Intentando añadir clave: {Key}", key);

        if (_data.TryGetValue(key, out var existingValue)) {
            
            _logger.Debug("Clave {Key} ya existe. Actualizando valor.", key);
            _data[key] = value; //update valor
            RefreshUsage(key);
            return Task.CompletedTask;
        }

        if (_data.Count >= _capacidad) {
            
            var oldestKey = _usageOrder.First!.Value;
            _logger.Information("Caché llena ({Capacidad}). Desalojando elemento menos usado: {OldestKey}", _capacidad, oldestKey);

            _usageOrder.RemoveFirst();
            _data.Remove(oldestKey);
        }

        _data.Add(key, value);
        _usageOrder.AddLast(key);
        _logger.Debug("Elemento añadido con éxito.");
        return Task.CompletedTask;
    }

    public Task<TValue?> Get(TKey key) {
        
        _logger.Debug("Buscando clave: {Key}", key);

        if (!_data.TryGetValue(key, out var value))
            return Task.FromResult<TValue?>(default);

        _logger.Debug("Clave {Key} encontrada. Rejuveneciendo prioridad...", key);
        RefreshUsage(key);
        return Task.FromResult<TValue?>(value);
    }

    public Task<bool> Remove(TKey key) {
        
        _logger.Debug("Intentando eliminar clave: {Key}", key);

        if (!_data.Remove(key))
            return Task.FromResult(false);

        _usageOrder.Remove(key);
        _logger.Debug("Clave {Key} eliminada correctamente de las estructuras.", key);
        return Task.FromResult(true);
    }

    public Task DisplayStatus() {
        
        _logger.Information("Capacidad: {DataCount}/{Capacidad}", _data.Count, _capacidad);
        _logger.Information("Historial de uso (Menos usado -> Más usado): {Order}", string.Join(" -> ", _usageOrder));
        return Task.CompletedTask;
    }

    public Task Clear() {
        
        _data.Clear();
        _usageOrder.Clear();
        _logger.Debug("Caché borrada por completo debido a un borrado masivo.");
        return Task.CompletedTask;
    }

    /// <summary> Mueve una clave a la última posición indicando que es la más recientemente usada </summary>
    private void RefreshUsage(TKey key) {
        _usageOrder.Remove(key);
        _usageOrder.AddLast(key);
    }
}