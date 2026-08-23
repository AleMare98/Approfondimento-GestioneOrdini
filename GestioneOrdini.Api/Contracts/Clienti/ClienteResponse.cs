namespace GestioneOrdini.Api.Contracts.Clienti;

public sealed record ClienteResponse(
    int Id,
    string Nome,
    string Email);