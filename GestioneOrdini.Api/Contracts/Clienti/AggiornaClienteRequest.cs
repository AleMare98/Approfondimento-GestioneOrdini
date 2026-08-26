namespace GestioneOrdini.Api.Contracts.Clienti;

public sealed record AggiornaClienteRequest(
    string Nome,
    string Email);