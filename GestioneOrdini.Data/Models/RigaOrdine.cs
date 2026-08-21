namespace GestioneOrdini.Data.Models;

public sealed class RigaOrdine
{
    public int IdOrdine { get; set; }
    public int IdProdotto { get; set; }
    public int Quantita { get; set; }
    public decimal PrezzoUnitario { get; set; }
    public Ordine Ordine { get; set; } = null!; // null! verra valorizzata da EF core
    public Prodotto Prodotto { get; set; } = null!;
}