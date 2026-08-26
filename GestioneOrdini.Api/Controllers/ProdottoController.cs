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
    public async Task<ActionResult<IReadOnlyList<ProdottoResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var prodotti = await _prodottoService.GetAllAsync(cancellationToken);
        return Ok(prodotti);   
    }
    
    [HttpGet("{idProdotto:int}")]
    public async Task<ActionResult<ProdottoResponse>> GetById(
        int idProdotto,
        CancellationToken cancellationToken)
    {
        var prodotto = await _prodottoService.GetByIdAsync(idProdotto, cancellationToken);
        return prodotto is null ? NotFound() : Ok(prodotto);
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
        return prodotto is null ? NotFound() : Ok(prodotto);
    }
    
    [HttpDelete("{idProdotto:int}")]
    public async Task<ActionResult> DeleteById(int idProdotto, CancellationToken cancellationToken)
    {
        var stato = await _prodottoService.DeleteAsync(idProdotto, cancellationToken);
        return stato ? NoContent() : NotFound();
    }
}