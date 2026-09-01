using GestioneOrdini.Api.Services;
using Microsoft.AspNetCore.Mvc;
using GestioneOrdini.Api.Contracts.Prodotti;

namespace GestioneOrdini.Api.Controllers;

[ApiController]
[Route("api/prodotti")]

public sealed class ProdottoController : ControllerBase
{
    private readonly ProdottoService _prodottoService;
    
    public ProdottoController(ProdottoService prodottoService)
    {
        _prodottoService = prodottoService;
    }

    [HttpGet]
    // AI - inizio filtro e paginazione prodotti
    public async Task<ActionResult<IReadOnlyList<ProdottoResponse>>> GetAll(
        string? nome,
        CancellationToken cancellationToken,
        [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue)]
        int pagina = 1,

        [System.ComponentModel.DataAnnotations.Range(1, 100)]
        int dimensionePagina = 10)
    {
        var prodotti = await _prodottoService.SearchAsync(
            nome,
            pagina,
            dimensionePagina,
            cancellationToken);

        return Ok(prodotti);
    }
    // AI - fine filtro e paginazione prodotti
    
    [HttpGet("{idProdotto:int}")]
    public async Task<ActionResult<ProdottoResponse>> GetById(
        int idProdotto,
        CancellationToken cancellationToken)
    {
        var prodotto = await _prodottoService.GetByIdAsync(idProdotto, cancellationToken);
        return Ok(prodotto);
    }
    
    [HttpPost]
    public async Task<ActionResult<ProdottoResponse>> Create(
        CreaProdottoRequest request,
        CancellationToken cancellationToken)
    {
        var prodotto = await _prodottoService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(
            nameof(GetById),
            new { idProdotto = prodotto.Id },
            prodotto);
    }
    
    [HttpPut("{idProdotto:int}")]
    public async Task<ActionResult<ProdottoResponse>> UpdateById(
        int idProdotto,
        AggiornaProdottoRequest request,
        CancellationToken cancellationToken)
    {
        var prodotto = await _prodottoService.UpdateAsync(idProdotto, request, cancellationToken);
        return Ok(prodotto);
    }
    
    [HttpDelete("{idProdotto:int}")]
    public async Task<ActionResult> DeleteById(int idProdotto, CancellationToken cancellationToken)
    {
        await _prodottoService.DeleteAsync(idProdotto, cancellationToken);
        return NoContent();
    }
}
