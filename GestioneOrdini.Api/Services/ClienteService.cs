using GestioneOrdini.Api.Contracts.Clienti;
using GestioneOrdini.Api.Errors;
using GestioneOrdini.Data.Models;
using GestioneOrdini.Data.Repositories;

namespace GestioneOrdini.Api.Services;

public sealed class ClienteService
{
    private readonly IClienteRepository _clienteRepository;

    public ClienteService(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<IReadOnlyList<ClienteResponse>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var clienti = await _clienteRepository.GetAllAsync(cancellationToken);

        return clienti.Select(ToResponse).ToList();
    }
    
    public async Task<ClienteResponse> GetByIdAsync(
        int idCliente,
        CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.GetByIdAsync(
            idCliente,
            cancellationToken);
        
        if (cliente is null)
        {
            throw new ResourceNotFoundException($"Cliente con ID {idCliente} non trovato.");
        }

        return  ToResponse(cliente);
    }
    
    public async Task<ClienteResponse> CreateAsync(
        CreaClienteRequest request,
        CancellationToken cancellationToken)
    {
        var cliente = new Cliente
        {
            Nome = request.Nome,
            Email = request.Email
        };
        
        var clienteEsistente = await _clienteRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (clienteEsistente is not null)
        {
            throw new ResourceConflictException("Esiste già un cliente con questa email");
        }

        var idCliente = await _clienteRepository.AddAsync(
            cliente,
            cancellationToken);

        cliente.IdCliente = idCliente;

        return ToResponse(cliente);
    }

    public async Task<ClienteResponse> UpdateAsync(int idCliente, AggiornaClienteRequest request, CancellationToken cancellationToken)
    {
        var cliente = new Cliente
        {
            IdCliente = idCliente,
            Nome = request.Nome,
            Email = request.Email
        };
        
        var clienteEsistente = await _clienteRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (clienteEsistente is not null && clienteEsistente.IdCliente != idCliente)
        {
            throw new ResourceConflictException("Esiste giò un cliente con questa email");
        }
        
        var stato = await _clienteRepository.UpdateAsync(cliente, cancellationToken);
        
        if (!stato)
        {
            throw new ResourceNotFoundException($"Cliente con ID {idCliente} non trovato.");
        }
        
        return ToResponse(cliente);
    }
    
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var stato = await _clienteRepository.DeleteAsync(id, cancellationToken);
        if (!stato)
        {
            throw new ResourceNotFoundException($"Cliente con ID {id} non trovato.");
        }
        
    }

    private static ClienteResponse ToResponse(Cliente cliente) => new(
        cliente.IdCliente,
        cliente.Nome,
        cliente.Email);
}
