namespace GestioneOrdini.Api.Errors;

public sealed class ResourceConflictException : Exception
{
    public ResourceConflictException (string message) : base(message)
    {
        
    }
}