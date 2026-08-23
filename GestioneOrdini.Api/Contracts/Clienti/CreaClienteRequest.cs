namespace GestioneOrdini.Api.Contracts.Clienti;

public sealed record CreaClienteRequest(
    string Nome,
    string Email);