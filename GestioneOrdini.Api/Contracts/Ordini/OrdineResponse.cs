namespace GestioneOrdini.Api.Contracts.Ordini;

public sealed record OrdineResponse(
    int Id,
    DateTime DataOrdine,
    ClienteSintesiResponse Cliente,
    IReadOnlyList<RigaOrdineResponse> Righe,
    decimal Totale);

public sealed record ClienteSintesiResponse(
    int Id,
    string Nome);

public sealed record RigaOrdineResponse(
    int IdProdotto,
    string NomeProdotto,
    int Quantita,
    decimal PrezzoUnitario,
    decimal TotaleRiga);
    

public sealed record EfOrdineResponse(
    int Id,
    DateTime DataOrdine,
    ClienteOrdineResponse Cliente,
    IReadOnlyList<RigaOrdineResponse> Righe,
    decimal Totale);

public sealed record ClienteOrdineResponse(
    int Id,
    string Nome);

