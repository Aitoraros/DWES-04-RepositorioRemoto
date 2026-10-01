using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepositorioRemoto.Entity;

/// <summary>
/// Representación de User para la BD
/// </summary>
[Table("Users")]
public class UserEntity
{
    /// <summary>
    /// ID del user, no incremental porque lo gestiona la API
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string Alias { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string DireccionCalle { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string DireccionSuite { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string DireccionCiudad { get; set; } = string.Empty;

    [Required]
    public string DireccionCodigoPostal { get; set; } = string.Empty;

    [Required]
    public double DireccionLatitud { get; set; }

    [Required]
    public double DireccionLongitud { get; set; }

    [Required]
    [MaxLength(25)]
    public string Telefono { get; set; } = string.Empty;

    [Required]
    public string Web { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string CompaniaNombre { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string CompaniaEslogan { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string CompaniaBs { get; set; } = string.Empty;
}