using Dapper;
using GestioneOrdini.Data.Models;
using MySqlConnector;

namespace GestioneOrdini.Data.Repositories;

public sealed class OrdineRepository
{
    private readonly string _connectionString;

    public OrdineRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<int> AddAsync(
        NuovoOrdine ordine,
        CancellationToken cancellationToken = default)
    {
        if (ordine.Righe.Count == 0)
        {
            throw new ArgumentException(
                "Un ordine deve contenere almeno una riga.",
                nameof(ordine));
        }

        const string insertOrdineSql = """
            INSERT INTO Ordini (IdCliente)
            VALUES (@IdCliente);

            SELECT LAST_INSERT_ID();
            """;

        const string insertRigaSql = """
            INSERT INTO RigheOrdine
                (IdOrdine, IdProdotto, Quantita, PrezzoUnitario)
            VALUES
                (@IdOrdine, @IdProdotto, @Quantita, @PrezzoUnitario);
            """;

        await using var connection =
            new MySqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken);

        await using var transaction =
            await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            var idOrdine = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(
                    insertOrdineSql,
                    new { ordine.IdCliente },
                    transaction: transaction,
                    cancellationToken: cancellationToken));

            foreach (var riga in ordine.Righe)
            {
                await connection.ExecuteAsync(
                    new CommandDefinition(
                        insertRigaSql,
                        new
                        {
                            IdOrdine = idOrdine,
                            riga.IdProdotto,
                            riga.Quantita,
                            riga.PrezzoUnitario
                        },
                        transaction: transaction,
                        cancellationToken: cancellationToken));
            }

            await transaction.CommitAsync(cancellationToken);

            return idOrdine;
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }
}