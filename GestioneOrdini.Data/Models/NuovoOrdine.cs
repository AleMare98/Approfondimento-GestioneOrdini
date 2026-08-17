namespace GestioneOrdini.Data.Models;

public sealed record NuovoOrdine(
    int IdCliente,
    IReadOnlyList<NuovaRigaOrdine> Righe);