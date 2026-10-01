using System.Globalization;
using RepositorioRemoto.Dto;
using RepositorioRemoto.Entity;
using RepositorioRemoto.Models;

namespace RepositorioRemoto.Mappers;

/// <summary>
/// 
/// </summary>
public static class UserMapper {

    /// <summary>
    /// Función de extensión encargada de mapear un UserDto -> User
    /// </summary>
    /// <param name="dto">Dto. a mapear</param>
    /// <returns>User mapeado</returns>
    public static User ToModel(this UserDto dto) => new() {
        Id = dto.Id,
        Alias = dto.Alias,
        Nombre = dto.Nombre,
        Email = dto.Email,
        DireccionCalle = dto.Direccion.Calle,
        DireccionSuite = dto.Direccion.Suite,
        DireccionCiudad = dto.Direccion.Ciudad,
        DireccionCodigoPostal = dto.Direccion.CodigoPostal,
        DireccionLatitud = double.Parse(dto.Direccion.Coordenadas.Latitud, CultureInfo.InvariantCulture),
        DireccionLongitud = double.Parse(dto.Direccion.Coordenadas.Longitud, CultureInfo.InvariantCulture),
        Telefono = dto.Telefono,
        Web = dto.Web,
        CompaniaNombre = dto.Compania.Nombre,
        CompaniaEslogan = dto.Compania.Eslogan,
        CompaniaBs = dto.Compania.Bs
    };

    /// <summary>
    /// Función de extensión encargada de mapear un User -> UserDto
    /// </summary>
    /// <param name="user">User a mapear</param>
    /// <returns>UserDto mapeado</returns>
    public static UserDto ToDto(this User user) => new(
        user.Id,
        user.Nombre,
        user.Alias,
        user.Email,
        new DireccionDto(
            user.DireccionCalle,
            user.DireccionSuite,
            user.DireccionCiudad,
            user.DireccionCodigoPostal,
            new GeoDto(
                user.DireccionLatitud.ToString(CultureInfo.InvariantCulture),
                user.DireccionLongitud.ToString(CultureInfo.InvariantCulture)
            )
        ),
        user.Telefono,
        user.Web,
        new CompaniaDto(
            user.CompaniaNombre, 
            user.CompaniaEslogan, 
            user.CompaniaBs
        )
    );

    /// <summary>
    /// Función de extensión encargada de mapear un CreateUserRequest -> User
    /// </summary>
    /// <param name="request">La CreateUserRequest a mapear</param>
    /// <param name="id">Id del User</param>
    /// <returns>User mapeado</returns>
    public static User ToModel(this CreateUserRequest request, int id = 0) => new() {
        Id = id,
        Alias = request.Alias,
        Nombre = request.Nombre,
        Email = request.Email,
        DireccionCalle = request.DireccionCalle,
        DireccionSuite = request.DireccionSuite,
        DireccionCiudad = request.DireccionCiudad,
        DireccionCodigoPostal = request.DireccionCodigoPostal,
        DireccionLatitud = request.DireccionLatitud,
        DireccionLongitud = request.DireccionLongitud,
        Telefono = request.Telefono,
        Web = request.Web,
        CompaniaNombre = request.CompaniaNombre,
        CompaniaEslogan = request.CompaniaEslogan,
        CompaniaBs = request.CompaniaBs
    };

    /// <summary>
    /// Función de extensión encargada de mapear un UpdateUserRequest -> User
    /// </summary>
    /// <param name="request">La UpdateUserRequest a mapear</param>
    /// <returns>User mapeado</returns>
    public static User ToModel(this UpdateUserRequest request) => new() {
        Id = request.Id,
        Alias = request.Alias,
        Nombre = request.Nombre,
        Email = request.Email,
        DireccionCalle = request.DireccionCalle,
        DireccionSuite = request.DireccionSuite,
        DireccionCiudad = request.DireccionCiudad,
        DireccionCodigoPostal = request.DireccionCodigoPostal,
        DireccionLatitud = request.DireccionLatitud,
        DireccionLongitud = request.DireccionLongitud,
        Telefono = request.Telefono,
        Web = request.Web,
        CompaniaNombre = request.CompaniaNombre,
        CompaniaEslogan = request.CompaniaEslogan,
        CompaniaBs = request.CompaniaBs
    };
    
    /// <summary>
    /// Función de extensión encargada de mapear un UserEntity -> User
    /// </summary>
    /// <param name="entity">Entidad a mapear</param>
    /// <returns>User mapeado</returns>
    public static User ToModel(this UserEntity entity) => new() {
        Id = entity.Id,
        Alias = entity.Alias,
        Nombre = entity.Nombre,
        Email = entity.Email,
        DireccionCalle = entity.DireccionCalle,
        DireccionSuite = entity.DireccionSuite,
        DireccionCiudad = entity.DireccionCiudad,
        DireccionCodigoPostal = entity.DireccionCodigoPostal,
        DireccionLatitud = entity.DireccionLatitud,
        DireccionLongitud = entity.DireccionLongitud,
        Telefono = entity.Telefono,
        Web = entity.Web,
        CompaniaNombre = entity.CompaniaNombre,
        CompaniaEslogan = entity.CompaniaEslogan,
        CompaniaBs = entity.CompaniaBs
    };

    /// <summary>
    /// Función de extensión encargada de mapear un User -> UserEntity
    /// </summary>
    /// <param name="model">Modelo a mapear</param>
    /// <returns>Entidad mapeada</returns>
    public static UserEntity ToEntity(this User model) => new() {
        Id = model.Id,
        Alias = model.Alias,
        Nombre = model.Nombre,
        Email = model.Email,
        DireccionCalle = model.DireccionCalle,
        DireccionSuite = model.DireccionSuite,
        DireccionCiudad = model.DireccionCiudad,
        DireccionCodigoPostal = model.DireccionCodigoPostal,
        DireccionLatitud = model.DireccionLatitud,
        DireccionLongitud = model.DireccionLongitud,
        Telefono = model.Telefono,
        Web = model.Web,
        CompaniaNombre = model.CompaniaNombre,
        CompaniaEslogan = model.CompaniaEslogan,
        CompaniaBs = model.CompaniaBs
    };
}