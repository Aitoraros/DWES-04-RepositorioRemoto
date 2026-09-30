using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositorioRemoto.Errors.Api;
using RepositorioRemoto.Errors.Common;
using RepositorioRemoto.Errors.User;

namespace RepositorioRemoto.Endpoints;

/// <summary>Traduce errores de dominio a respuestas HTTP.</summary>
public static class DomainErrorExtensions
{
    /// <summary>
    /// Convierte un error de dominio en su respuesta HTTP equivalente.
    /// </summary>
    /// <param name="error">Error de dominio tipado.</param>
    /// <returns>
    /// 404 <c>UserErrors.NotFound</c> · 400 <c>UserErrors.Validation</c> (+ <c>errors</c>) ·
    /// 502 <c>ApiErrors.BadResponse</c> · 503 <c>ApiErrors.Unavailable</c> ·
    /// 500 <c>UserErrors.ExportFailure</c>/desconocidos.
    /// </returns>
    public static IActionResult ToHttpResult(this DomainError error) => error switch
    {
        UserErrors.NotFound e      => new NotFoundObjectResult(new { message = e.Message }),
        UserErrors.Validation e    => new BadRequestObjectResult(new { message = e.Message, errors = e.Errors }),
        UserErrors.ExportFailure e => new ObjectResult(new { message = e.Message }) { StatusCode = StatusCodes.Status500InternalServerError },
        ApiErrors.BadResponse e    => new ObjectResult(new { message = e.Message }) { StatusCode = StatusCodes.Status502BadGateway },
        ApiErrors.Unavailable e    => new ObjectResult(new { message = e.Message }) { StatusCode = StatusCodes.Status503ServiceUnavailable },
        _                          => new ObjectResult(new { message = error.Message }) { StatusCode = StatusCodes.Status500InternalServerError }
    };
}