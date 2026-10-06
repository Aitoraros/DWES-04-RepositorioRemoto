using RepositorioRemoto.Entity;

namespace RepositorioRemoto.Repositories;

/// <summary>
/// Interfaz a implementar por los repositorios
/// </summary>
public interface IUserRepository {
    
    /// <summary>
    /// Obtener todos
    /// </summary>
    Task<IEnumerable<UserEntity>> GetAllAsync();
    
    /// <summary>
    /// Obtener usuario en base a un ID
    /// </summary>
    Task<UserEntity?> GetByIdAsync(int id);
    
    /// <summary>
    /// Crear un usuario pasado
    /// </summary>
    Task<UserEntity> CreateAsync(UserEntity user);
    
    /// <summary>
    /// Actualizar un usuario pasado
    /// </summary>
    Task<UserEntity?> UpdateAsync(UserEntity user);
    
    /// <summary>
    /// Elimina a un user en base al ID
    /// </summary>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Inserta una lista de usuarios
    /// </summary>
    Task InsertRangeAsync(IEnumerable<UserEntity> users);
    
    /// <summary>
    /// Elimina todos los datos
    /// </summary>
    Task DeleteAllAsync();
}