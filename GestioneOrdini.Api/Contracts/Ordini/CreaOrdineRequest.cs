using System.ComponentModel.DataAnnotations;

namespace GestioneOrdini.Api.Contracts.Ordini;

// EF
public sealed record CreaOrdineRequest(
    [property: Range(1, int.MaxValue)]
    int IdCliente,
    [property: Required]
    [property: MinLength(1)]
    IReadOnlyList<CreaRigaOrdineRequest> Righe);

public sealed record CreaRigaOrdineRequest(
    [property: Range(1, int.MaxValue)]
    int IdProdotto,
    [property: Range(1, int.MaxValue)]
    int Quantita);