using Dapper;
using GestioneOrdini.Data.Models;
using MySqlConnector;

namespace GestioneOrdini.Data.Repositories;

public sealed class ClienteRepository : IClienteRepository
{
    private readonly string _connectionString;

    public ClienteRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<Cliente?> GetByIdAsync(
        int idCliente,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
                           SELECT IdCliente, Nome, Email
                           FROM Clienti
                           WHERE IdCliente = @IdCliente;
                           """;

        var command = new CommandDefinition(
            sql,
            new { IdCliente = idCliente },
            cancellationToken: cancellationToken);

        await using var connection = new MySqlConnection(_connectionString);

        return await connection.QuerySingleOrDefaultAsync<Cliente>(command);
    }

    public async Task<Cliente?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        const string sql = """
                           SELECT IdCliente, Nome, Email
                           FROM Clienti
                           WHERE Email = @Email;
                           """;
        
        var command = new CommandDefinition(
            sql,
            new { Email = email },
            cancellationToken: cancellationToken);
        
        await using var connection = new MySqlConnection(_connectionString);
        
        return await connection.QuerySingleOrDefaultAsync<Cliente>(command);
    }

    public async Task<IReadOnlyList<Cliente>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
                            SELECT IdCliente AS ID, Nome, Email
                            FROM Clienti
                            ORDER BY Nome;
                           """;
        
        var command = new CommandDefinition(sql, cancellationToken: cancellationToken);
        
        await using var connection = new MySqlConnection(_connectionString);

        var lista =  await connection.QueryAsync<Cliente>(command);
        
        return lista.ToList();
        
    }

    public async Task<int> AddAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        
        
        const string sql = """
                            INSERT INTO Clienti (Nome, Email) 
                            VALUES (@Nome, @Email); 
                            SELECT LAST_INSERT_ID(); 
                           """;
        var command = new CommandDefinition(sql, new { cliente.Nome, cliente.Email }, cancellationToken: cancellationToken);
        
        await using var connection = new MySqlConnection(_connectionString);
        return await connection.ExecuteScalarAsync<int>(command);
    }
    
}