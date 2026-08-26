using GestioneOrdini.Api.Contracts.Ordini;
using GestioneOrdini.Data.Models;
using GestioneOrdini.Data.Repositories;

namespace GestioneOrdini.Api.Services;

public sealed class OrdineService
{
    private readonly IOrdineEfRepository _ordineEfRepository;
    private readonly IProdottoRepository _prodottoRepository;
    private readonly IClienteRepository _clienteRepository;
    
    public OrdineService(IOrdineEfRepository ordineEfRepository, IProdottoRepository prodottoRepository, IClienteRepository clienteRepository)
    {
        _ordineEfRepository = ordineEfRepository;
        _prodottoRepository = prodottoRepository;
        _clienteRepository = clienteRepository;
    }
    
    public async Task<OrdineResponse?> CreateAsync(CreaOrdineRequest request, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.GetByIdAsync(request.IdCliente, cancellationToken);

        if (cliente is null)
        {
            return null;
        }

        var righe = new List<NuovaRigaOrdine>();

        foreach (var rigaRequest in request.Righe)
        {
            var prodotto = await _prodottoRepository.GetByIdAsync(rigaRequest.IdProdotto, cancellationToken);

            if (prodotto is null)
            {
                return null;
            }

            righe.Add(new NuovaRigaOrdine(rigaRequest.IdProdotto, rigaRequest.Quantita, prodotto.Prezzo));
        }

        var nuovoOrdine = new NuovoOrdine(request.IdCliente, righe);

        var idOrdine = await _ordineEfRepository.AddAsync(nuovoOrdine, cancellationToken);

        var ordine = await _ordineEfRepository.GetByIdAsync(idOrdine, cancellationToken) ?? throw new InvalidOperationException(
            "Ordine creato ma non trovato durante la rilettura.");

        return ToResponse(ordine);
    }
    public async Task<OrdineResponse?> GetByIdAsync(int idOrdine, CancellationToken cancellationToken)
    {
        var ordine = await _ordineEfRepository.GetByIdAsync(idOrdine, cancellationToken);

        return ordine is null ? null : ToResponse(ordine);
    }

    private static OrdineResponse ToResponse(Ordine ordine)
    {
        var righe = ordine.Righe
            .Select(riga => new RigaOrdineResponse(
                riga.IdProdotto,
                riga.Prodotto.Nome,
                riga.Quantita,
                riga.PrezzoUnitario,
                riga.Quantita * riga.PrezzoUnitario))
            .ToList();

        return new OrdineResponse(
            ordine.IdOrdine,
            ordine.DataOrdine,
            new ClienteSintesiResponse(
                ordine.Cliente.IdCliente,
                ordine.Cliente.Nome),
            righe,
            righe.Sum(riga => riga.TotaleRiga));
    }
 
}