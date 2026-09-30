namespace RepositorioRemoto.Errors.Common;

public abstract record DomainError(string Message)
{
    /// <summary>Representación formateada del error.</summary>
    public override string ToString() => $"{GetType().Name}: {Message}";
}