namespace GestioneOrdini.Api.Contracts.Prodotti;

public sealed record AggiornaProdottoRequest(
    string Nome,
    decimal Prezzo);