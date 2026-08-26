using GestioneOrdini.Data.Models;

namespace GestioneOrdini.Data.Repositories;

public interface IProdottoRepository
{
    //*** public è implicito nei membri di un’interfaccia ***
    Task<Prodotto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Prodotto>> GetAllAsync(CancellationToken cancellationToken = default);
    // AI - inizio filtro e paginazione prodotti
    Task<IReadOnlyList<Prodotto>> SearchAsync(
        string? nome,
        int pagina,
        int dimensionePagina,
        CancellationToken cancellationToken = default);
    // AI - fine filtro e paginazione prodotti
    Task<int> AddAsync(Prodotto prodotto, CancellationToken cancellationToken = default);
    
    Task<bool> UpdateAsync(Prodotto prodotto, CancellationToken cancellationToken = default);
    
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
