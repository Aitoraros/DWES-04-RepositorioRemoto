namespace RepositorioRemoto.Dto;

public record UpdateUserRequest(
    int Id,
    string Nombre,
    string Alias,
    string Email,
    string DireccionCalle,
    string DireccionSuite,
    string DireccionCiudad,
    string DireccionCodigoPostal,
    double DireccionLatitud,
    double DireccionLongitud,
    string Telefono,
    string Web,
    string CompaniaNombre,
    string CompaniaEslogan,
    string CompaniaBs
);