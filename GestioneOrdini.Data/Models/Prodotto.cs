namespace GestioneOrdini.Data.Models;

public sealed class Prodotto
{
    public int IdProdotto { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Prezzo { get; set; }
}