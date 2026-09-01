
using GestioneOrdini.Api.Contracts.Prodotti;
using GestioneOrdini.Api.Errors;
using GestioneOrdini.Data.Models;
using GestioneOrdini.Data.Repositories;


namespace GestioneOrdini.Api.Services;
public sealed class ProdottoService
{
    private readonly IProdottoRepository _prodottoRepository;

    public ProdottoService(IProdottoRepository prodottoRepository)
    {
        _prodottoRepository = prodottoRepository;
    }
    
    public async Task<IReadOnlyList<ProdottoResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var prodotti = await _prodottoRepository.GetAllAsync(cancellationToken);
        return prodotti.Select(ToResponse).ToList();
    }

    // AI - inizio filtro e paginazione prodotti
    public async Task<IReadOnlyList<ProdottoResponse>> SearchAsync(
        string? nome,
        int pagina,
        int dimensionePagina,
        CancellationToken cancellationToken)
    {
        var prodotti = await _prodottoRepository.SearchAsync(
            nome,
            pagina,
            dimensionePagina,
            cancellationToken);

        return prodotti.Select(ToResponse).ToList();
    }
    // AI - fine filtro e paginazione prodotti

    public async Task<ProdottoResponse> GetByIdAsync(int idProdotto, CancellationToken cancellationToken)
    {
        var prodotto = await _prodottoRepository.GetByIdAsync(idProdotto, cancellationToken);
        if (prodotto is null)
        {
            throw new ResourceNotFoundException($"prodotto con ID {idProdotto} non trovato.");
        }
        return ToResponse(prodotto);
    }

    public async Task<ProdottoResponse> CreateAsync(CreaProdottoRequest request, CancellationToken cancellationToken)
    {
        var prodotto = new Prodotto
        {
            Nome = request.Nome,
            Prezzo = request.Prezzo
        };
        var idProdotto = await _prodottoRepository.AddAsync(prodotto, cancellationToken);
        prodotto.IdProdotto = idProdotto;
        return ToResponse(prodotto);
    }

    public async Task<ProdottoResponse> UpdateAsync(int idProdotto, AggiornaProdottoRequest request,
        CancellationToken cancellationToken)
    {
        var prodotto = new Prodotto
        {
            IdProdotto = idProdotto,
            Nome = request.Nome,
            Prezzo = request.Prezzo
        };
        var stato = await _prodottoRepository.UpdateAsync(prodotto, cancellationToken);
        if (!stato)
        {
            throw new ResourceNotFoundException($"prodotto con ID {idProdotto} non trovato.");
        }

        return ToResponse(prodotto);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var stato = await _prodottoRepository.DeleteAsync(id, cancellationToken);
        if (!stato)
        {
            throw new ResourceNotFoundException($"prodotto con ID {id} non trovato.");
        }
        

    }
    private static ProdottoResponse ToResponse(Prodotto prodotto) => new(
        prodotto.IdProdotto,
        prodotto.Nome,
        prodotto.Prezzo);
}
