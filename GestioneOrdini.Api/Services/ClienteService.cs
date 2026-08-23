using GestioneOrdini.Api.Contracts.Clienti;
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
    
    public async Task<ClienteResponse?> GetByIdAsync(
        int idCliente,
        CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.GetByIdAsync(
            idCliente,
            cancellationToken);

        return cliente is null ? null : ToResponse(cliente);
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

        var idCliente = await _clienteRepository.AddAsync(
            cliente,
            cancellationToken);

        cliente.IdCliente = idCliente;

        return ToResponse(cliente);
    }

    private static ClienteResponse ToResponse(Cliente cliente) => new(
        cliente.IdCliente,
        cliente.Nome,
        cliente.Email);
}