namespace GestioneOrdini.Api.Contracts.Prodotti;

public sealed record ProdottoResponse (
    int Id,
    string Nome,
    decimal Prezzo);