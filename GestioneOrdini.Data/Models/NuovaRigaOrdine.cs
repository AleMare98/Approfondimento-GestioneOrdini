namespace GestioneOrdini.Data.Models;

public sealed record NuovaRigaOrdine(
    int IdProdotto,
    int Quantita,
    decimal PrezzoUnitario);