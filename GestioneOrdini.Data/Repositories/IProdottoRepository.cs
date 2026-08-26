using GestioneOrdini.Data.Models;

namespace GestioneOrdini.Data.Repositories;

public interface IProdottoRepository
{
    //*** public è implicito nei membri di un’interfaccia ***
    Task<Prodotto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Prodotto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<int> AddAsync(Prodotto prodotto, CancellationToken cancellationToken = default);
    
    Task<bool> UpdateAsync(Prodotto prodotto, CancellationToken cancellationToken = default);
    
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}