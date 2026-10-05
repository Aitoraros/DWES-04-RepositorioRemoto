namespace RepositorioRemoto.Cache.Common;


/// <summary>
/// Contrato para una caché genérica
/// </summary>
/// <typeparam name="TKey">Tipo de la clave</typeparam>
/// <typeparam name="TValue">Tipo del objeto a guardar</typeparam>
public interface ICache<in TKey, TValue> where TKey : notnull {
    
    /// <summary>
    /// Agrega un elemento a la caché. Si está llena (solo aplica a cachés en memoria), elimina el menos usado.
    /// </summary>
    Task Add(TKey key, TValue value);

    /// <summary>
    /// Obtiene un elemento de la caché
    /// </summary>
    Task<TValue?> Get(TKey key);

    /// <summary>
    /// Elimina un elemento de la caché
    /// </summary>
    Task<bool> Remove(TKey key);

    /// <summary>
    /// Muestra el estado de la caché
    /// </summary>
    Task DisplayStatus();

    /// <summary>
    /// Vacía la caché
    /// </summary>
    Task Clear();
}