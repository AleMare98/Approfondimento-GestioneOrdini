using GestioneOrdini.Api.Contracts.Clienti;
using GestioneOrdini.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestioneOrdini.Api.Controllers;

[ApiController]
[Route("api/clienti")]
public sealed class ClientiController : ControllerBase
{
    private readonly ClienteService _clienteService;

    public ClientiController(ClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ClienteResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var clienti = await _clienteService.GetAllAsync(cancellationToken);
        return Ok(clienti);
    }
    
    [HttpGet("{idCliente:int}")]
    public async Task<ActionResult<ClienteResponse>> GetById(
        int idCliente,
        CancellationToken cancellationToken)
    {
        var cliente = await _clienteService.GetByIdAsync(
            idCliente,
            cancellationToken);

        return cliente is null ? NotFound() : Ok(cliente);
    }
    
    [HttpPost]
    public async Task<ActionResult<ClienteResponse>> Create(
        CreaClienteRequest request,
        CancellationToken cancellationToken)
    {
        var cliente = await _clienteService.CreateAsync(
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { idCliente = cliente.Id },
            cliente);
    }
}