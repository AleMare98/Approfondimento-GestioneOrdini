using GestioneOrdini.Api.Contracts.Ordini;
using GestioneOrdini.Api.Errors;
using GestioneOrdini.Data.Models;
using GestioneOrdini.Data.Repositories;

namespace GestioneOrdini.Api.Services;

public sealed class OrdineService
{
    private readonly IOrdineEfRepository _ordineEfRepository;
    private readonly IProdottoRepository _prodottoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly ILogger<OrdineService> _logger;
    
    public OrdineService(IOrdineEfRepository ordineEfRepository, IProdottoRepository prodottoRepository, IClienteRepository clienteRepository, ILogger<OrdineService> logger)
    {
        _ordineEfRepository = ordineEfRepository;
        _prodottoRepository = prodottoRepository;
        _clienteRepository = clienteRepository;
        _logger = logger;
    }
    
    public async Task<OrdineResponse> CreateAsync(CreaOrdineRequest request, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.GetByIdAsync(request.IdCliente, cancellationToken);

        if (cliente is null)
        {
            throw new ResourceNotFoundException($"Cliente con ID {request.IdCliente} non trovato.");
        }

        var righe = new List<NuovaRigaOrdine>();

        foreach (var rigaRequest in request.Righe)
        {
            var prodotto = await _prodottoRepository.GetByIdAsync(rigaRequest.IdProdotto, cancellationToken);

            if (prodotto is null)
            {
                throw new ResourceNotFoundException($"Prodotto con ID {rigaRequest.IdProdotto} non trovato.");
            }

            righe.Add(new NuovaRigaOrdine(rigaRequest.IdProdotto, rigaRequest.Quantita, prodotto.Prezzo));
        }

        var nuovoOrdine = new NuovoOrdine(request.IdCliente, righe);

        var idOrdine = await _ordineEfRepository.AddAsync(nuovoOrdine, cancellationToken);

        var ordine = await _ordineEfRepository.GetByIdAsync(idOrdine, cancellationToken) ?? throw new InvalidOperationException(
            "Ordine creato ma non trovato durante la rilettura.");
        
        _logger.LogInformation(
            "Ordine {IdOrdine} creato per cliente {IdCliente} con {NumeroRighe} righe", ordine.IdOrdine, request.IdCliente, ordine.Righe.Count);

        return ToResponse(ordine);
    }
    public async Task<OrdineResponse> GetByIdAsync(int idOrdine, CancellationToken cancellationToken)
    {
        var ordine = await _ordineEfRepository.GetByIdAsync(idOrdine, cancellationToken);

        if (ordine is null)
        {
            throw new ResourceNotFoundException($"Ordine con ID {idOrdine} non trovato.");
        }

        return ToResponse(ordine);
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