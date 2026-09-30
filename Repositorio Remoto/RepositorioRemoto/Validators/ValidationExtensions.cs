using System.Text.RegularExpressions;

namespace RepositorioRemoto.Validators;

/// <summary>Funciones de extensión para validar campos de texto.</summary>
public static class ValidationExtensions
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

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
}