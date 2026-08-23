namespace GestioneOrdini.Api.Contracts.Prodotti;

public sealed record CreaProdottoRequest(
    string Nome,
    decimal Prezzo);