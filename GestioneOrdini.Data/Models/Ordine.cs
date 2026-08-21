namespace GestioneOrdini.Data.Models;

public sealed class Ordine
{
    public int IdOrdine { get; set; }
    public int IdCliente { get; set; }
    public DateTime DataOrdine { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public ICollection<RigaOrdine> Righe { get; set; } = new List<RigaOrdine>();
}