using System.Text.Json.Serialization;

namespace RepositorioRemoto.Dto;

public record UserDto(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Nombre,
    [property: JsonPropertyName("username")] string Alias,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("address")] DireccionDto Direccion,
    [property: JsonPropertyName("phone")] string Telefono,
    [property: JsonPropertyName("website")] string Web,
    [property: JsonPropertyName("company")] CompaniaDto Compania
);

public record DireccionDto(
    [property: JsonPropertyName("street")] string Calle,
    [property: JsonPropertyName("suite")] string Suite,
    [property: JsonPropertyName("city")] string Ciudad,
    [property: JsonPropertyName("zipcode")] string CodigoPostal,
    [property: JsonPropertyName("geo")] GeoDto Coordenadas
);

public record GeoDto(
    [property: JsonPropertyName("lat")] string Latitud,
    [property: JsonPropertyName("lng")] string Longitud
);

public record CompaniaDto(
    [property: JsonPropertyName("name")] string Nombre,
    [property: JsonPropertyName("catchPhrase")] string Eslogan,
    [property: JsonPropertyName("bs")] string Bs
);