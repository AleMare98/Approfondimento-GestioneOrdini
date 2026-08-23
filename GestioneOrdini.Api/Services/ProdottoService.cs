
using GestioneOrdini.Api.Contracts.Prodotti;
using GestioneOrdini.Data.Models;
using GestioneOrdini.Data.Repositories;
using Microsoft.AspNetCore.Authentication.BearerToken;


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

    public async Task<ProdottoResponse?> GetByIdAsync(int idProdotto, CancellationToken cancellationToken)
    {
        var prodotto = await _prodottoRepository.GetByIdAsync(idProdotto, cancellationToken);
        return prodotto is null ? null : ToResponse(prodotto);
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
    private static ProdottoResponse ToResponse(Prodotto prodotto) => new(
        prodotto.IdProdotto,
        prodotto.Nome,
        prodotto.Prezzo);
}