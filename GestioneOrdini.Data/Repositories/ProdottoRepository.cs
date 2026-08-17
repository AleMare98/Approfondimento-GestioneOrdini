using Dapper;
using GestioneOrdini.Data.Models;
using MySqlConnector;

namespace GestioneOrdini.Data.Repositories;

public sealed class ProdottoRepository : IProdottoRepository
{
    private readonly string _connectionString;

    public ProdottoRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    
    public async Task<Prodotto?> GetByIdAsync(int idProdotto, CancellationToken cancellationToken = default)
    {
        const string sql = """
                            SELECT IdProdotto, Nome, Prezzo
                            FROM Prodotti
                            WHERE IdProdotto = @IdProdotto;
                           """;
        var command = new CommandDefinition(sql,
            new { IdProdotto = idProdotto },
            cancellationToken: cancellationToken);

        await using var connection = new MySqlConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<Prodotto>(command);
    }

    public async Task<IReadOnlyList<Prodotto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
                            SELECT IdProdotto, Nome, Prezzo
                            FROM Prodotti
                            ORDER BY Nome;
                           """;
        
        var command = new CommandDefinition(sql, cancellationToken: cancellationToken);
        
        await using var connection = new MySqlConnection(_connectionString);
        
        var lista = await connection.QueryAsync<Prodotto>(command);

        return lista.ToList();
    }

    public async Task<int> AddAsync(Prodotto prodotto, CancellationToken cancellationToken = default)
    {
         
        const string sql = """
                            INSERT INTO Prodotti (Nome, Prezzo) 
                            VALUES (@Nome, @Prezzo); 
                            SELECT LAST_INSERT_ID(); 
                           """;
        var command = new CommandDefinition(sql, new { prodotto.Nome, prodotto.Prezzo }, cancellationToken: cancellationToken);
        
        await using var connection = new MySqlConnection(_connectionString);
        return await connection.ExecuteScalarAsync<int>(command);
    }
}