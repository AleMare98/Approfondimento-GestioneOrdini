using GestioneOrdini.Data.Models;

namespace GestioneOrdini.Data.Repositories;

public interface IClienteRepository
{
    //*** public è implicito nei membri di un’interfaccia ***
    Task<Cliente?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Cliente?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Cliente>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<int> AddAsync(Cliente cliente, CancellationToken cancellationToken = default);
    
    Task<bool> UpdateAsync(Cliente cliente, CancellationToken cancellationToken = default);
    
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

}