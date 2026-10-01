using System.Text.RegularExpressions;

namespace RepositorioRemoto.Validators;

/// <summary>Funciones de extensión para validar campos de texto.</summary>
public static class ValidationExtensions
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    private static readonly Regex TelefonoRegex = new(@"^[0-9+\-().\sxX]+$");
    private static readonly Regex CodigoPostalRegex = new(@"^\d{5}(-\d{4})?$");
    private static readonly Regex WebRegex = new(@"^(https?://)?[^\s/]+\.[^\s/]+(/\S*)?$");

    /// <summary>Comprueba que el texto no está vacío y tiene una longitud entre min y max.</summary>
    /// <param name="value">Texto a comprobar.</param>
    /// <param name="min">Longitud mínima.</param>
    /// <param name="max">Longitud máxima.</param>
    /// <returns>true si es válido.</returns>
    public static bool HasLengthBetween(this string? value, int min, int max) =>
        !string.IsNullOrWhiteSpace(value) && value.Trim().Length >= min && value.Trim().Length <= max;

    /// <summary>Nombre obligatorio, entre 2 y 50 caracteres.</summary>
    public static bool IsNombreValid(this string? nombre) => nombre.HasLengthBetween(2, 50);

    /// <summary>Alias obligatorio, entre 3 y 20 caracteres y sin espacios.</summary>
    public static bool IsAliasValid(this string? alias) =>
        alias.HasLengthBetween(3, 20) && !alias!.Contains(' ');

    /// <summary>Email obligatorio con formato xxx@xxx.xxx.</summary>
    public static bool IsEmailValid(this string? email) =>
        !string.IsNullOrWhiteSpace(email) && EmailRegex.IsMatch(email);
    
    /// <summary>Campo opcional: vacío o con como máximo <paramref name="max"/> caracteres.</summary>
    public static bool HasMaxLength(this string? value, int max) =>
        string.IsNullOrWhiteSpace(value) || value.Trim().Length <= max;

    /// <summary>Latitud válida: entre -90 y 90.</summary>
    public static bool IsLatitudValid(this double latitud) => latitud is >= -90 and <= 90;

    /// <summary>Longitud válida: entre -180 y 180.</summary>
    public static bool IsLongitudValid(this double longitud) => longitud is >= -180 and <= 180;

    /// <summary>Campo opcional: vacío o con formato de teléfono (máximo 25 caracteres).</summary>
    public static bool IsTelefonoValid(this string? telefono) =>
        string.IsNullOrWhiteSpace(telefono) ||
        (telefono.Trim().Length <= 25 && TelefonoRegex.IsMatch(telefono.Trim()));

    /// <summary>Campo opcional: vacío o con formato 12345 / 12345-6789.</summary>
    public static bool IsCodigoPostalValid(this string? codigoPostal) =>
        string.IsNullOrWhiteSpace(codigoPostal) || CodigoPostalRegex.IsMatch(codigoPostal.Trim());

    /// <summary>Campo opcional: vacío o con formato de web (dominio.com, con o sin https://).</summary>
    public static bool IsWebValid(this string? web) =>
        string.IsNullOrWhiteSpace(web) || WebRegex.IsMatch(web.Trim());
}