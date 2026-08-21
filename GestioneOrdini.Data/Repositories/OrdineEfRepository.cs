using GestioneOrdini.Data.Data;
using GestioneOrdini.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GestioneOrdini.Data.Repositories;

// ** AI **
public sealed class OrdineEfRepository : IOrdineEfRepository
{
    private readonly GestioneOrdiniDbContext _dbContext;

    public OrdineEfRepository(GestioneOrdiniDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> AddAsync(
        NuovoOrdine nuovoOrdine,
        CancellationToken cancellationToken = default)
    {
        if (nuovoOrdine.Righe.Count == 0)
        {
            throw new ArgumentException(
                "Un ordine deve contenere almeno una riga.",
                nameof(nuovoOrdine));
        }

        var ordine = new Ordine
        {
            IdCliente = nuovoOrdine.IdCliente,
            Righe = nuovoOrdine.Righe
                .Select(riga => new RigaOrdine
                {
                    IdProdotto = riga.IdProdotto,
                    Quantita = riga.Quantita,
                    PrezzoUnitario = riga.PrezzoUnitario
                })
                .ToList()
        };

        await _dbContext.Ordini.AddAsync(ordine, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ordine.IdOrdine;
    }

    public Task<Ordine?> GetByIdAsync(
        int idOrdine,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Ordini
            .AsNoTracking()
            .Include(ordine => ordine.Cliente)
            .Include(ordine => ordine.Righe)
                .ThenInclude(riga => riga.Prodotto)
            .SingleOrDefaultAsync(
                ordine => ordine.IdOrdine == idOrdine,
                cancellationToken);
    }
}
