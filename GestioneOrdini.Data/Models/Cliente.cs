namespace GestioneOrdini.Data.Models;

public sealed class Cliente
{
    public int IdCliente { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}