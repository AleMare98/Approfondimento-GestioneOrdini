namespace GestioneOrdini.Api.Contracts.Ordini;

// EF
public sealed record CreaOrdineRequest(
    int IdCliente,
    IReadOnlyList<CreaRigaOrdineRequest> Righe);

public sealed record CreaRigaOrdineRequest(
    int IdProdotto,
    int Quantita);