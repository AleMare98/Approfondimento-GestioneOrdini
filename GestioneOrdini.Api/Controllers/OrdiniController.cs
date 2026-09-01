using Microsoft.AspNetCore.Mvc;
using GestioneOrdini.Api.Contracts.Ordini;
using GestioneOrdini.Api.Services;

namespace GestioneOrdini.Api.Controllers;

[ApiController]
[Route("api/ordini")]
// EF
public sealed class OrdiniController : ControllerBase
{
    private readonly OrdineService _ordineService;
    
    public OrdiniController(OrdineService ordineService)
    {
        _ordineService = ordineService;
    }
    
    [HttpGet("{idOrdine:int}")]
    public async Task<ActionResult<OrdineResponse>> GetById(
        int idOrdine,
        CancellationToken cancellationToken)
    {
        var ordine = await _ordineService.GetByIdAsync(idOrdine, cancellationToken);
        return  Ok(ordine);
    }
    
    [HttpPost]
    public async Task<ActionResult<OrdineResponse>> CreateAsync(
        CreaOrdineRequest request,
        CancellationToken cancellationToken)
    {
        var ordine = await _ordineService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), 
            new { idOrdine = ordine.Id }, ordine);
    }
}