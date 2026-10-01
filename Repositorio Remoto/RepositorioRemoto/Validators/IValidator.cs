using CSharpFunctionalExtensions;
using RepositorioRemoto.Errors.Common;

namespace RepositorioRemoto.Validators;

/// <summary>Contrato para validar entidades o DTOs.</summary>
public interface IValidator<T>
{
    /// <summary>Valida el objeto recibido.</summary>
    /// <param name="entity">Objeto a validar.</param>
    /// <returns>Success con el objeto si es válido, o Failure con un <see cref="DomainError"/> de validación.</returns>
    Result<T, DomainError> Validar(T entity);
}