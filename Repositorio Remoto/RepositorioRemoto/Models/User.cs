namespace RepositorioRemoto.Models;

/// <summary>
/// Modelo que representa un usuario.
/// </summary>
public record User {
    public int Id { get; init; }
    public string Alias { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DireccionCalle { get; set; } = string.Empty;
    public string DireccionSuite { get; set; } = string.Empty;
    public string DireccionCiudad { get; set; } = string.Empty;
    public string DireccionCodigoPostal { get; set; } = string.Empty;
    public double DireccionLatitud { get; set; }
    public double DireccionLongitud { get; set; }
    public string Telefono { get; set; } = string.Empty;
    public string Web { get; set; } = string.Empty;
    public string CompaniaNombre { get; set; } = string.Empty;
    public string CompaniaEslogan { get; set; } = string.Empty;
    public string CompaniaBs { get; set; } = string.Empty;
}