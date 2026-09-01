using System.ComponentModel.DataAnnotations;

namespace GestioneOrdini.Api.Contracts.Prodotti;

public sealed record AggiornaProdottoRequest(
    [property: Required]
    [property: StringLength(150, MinimumLength = 2)]
    string Nome,
    [property: Range(
        typeof(decimal),
        "0.01",
        "99999999.99")]
    decimal Prezzo);
