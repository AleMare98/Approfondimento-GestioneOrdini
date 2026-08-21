using GestioneOrdini.Data.Models;

namespace GestioneOrdini.Data.Repositories;

// ** AI **
public interface IOrdineEfRepository
{
    Task<int> AddAsync(
        NuovoOrdine nuovoOrdine,
        CancellationToken cancellationToken = default);

    Task<Ordine?> GetByIdAsync(
        int idOrdine,
        CancellationToken cancellationToken = default);
}
