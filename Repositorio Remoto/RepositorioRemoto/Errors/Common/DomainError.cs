namespace RepositorioRemoto.Errors.Common;

/// <summary>Error base del dominio.</summary>
public abstract record DomainError(string Message)
{
    public sealed override string ToString() => $"{GetType().Name}: {Message}";
}