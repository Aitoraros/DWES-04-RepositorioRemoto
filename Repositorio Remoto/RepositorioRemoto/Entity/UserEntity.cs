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
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("alias")]
    public string Alias { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("direccion_calle")]
    public string DireccionCalle { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    [Column("direccion_suite")]
    public string DireccionSuite { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    [Column("direccion_ciudad")]
    public string DireccionCiudad { get; set; } = string.Empty;

    [Required]
    [Column("direccion_codigo_postal")]
    public string DireccionCodigoPostal { get; set; } = string.Empty;

    [Required]
    [Column("direccion_latitud")]
    public double DireccionLatitud { get; set; }

    [Required]
    [Column("direccion_longitud")]
    public double DireccionLongitud { get; set; }

    [Required]
    [MaxLength(25)]
    [Column("telefono")]
    public string Telefono { get; set; } = string.Empty;

    [Required]
    [Column("web")]
    public string Web { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("compania_nombre")]
    public string CompaniaNombre { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    [Column("compania_eslogan")]
    public string CompaniaEslogan { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    [Column("compania_bs")]
    public string CompaniaBs { get; set; } = string.Empty;
}