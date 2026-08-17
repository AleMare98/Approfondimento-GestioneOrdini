using Dapper;
using GestioneOrdini.Data.Models;
using GestioneOrdini.Data.Repositories;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Xunit;

namespace GestioneOrdini.IntegrationTests;
// ** AI **
public sealed class ClienteRepositoryIntegrationTests
{
    [Fact]
    public async Task AddAsync_then_GetByEmailAsync_returns_created_client()
    {
        TestEnvironment.LoadFromLocalFile();

        var connectionString =
            Environment.GetEnvironmentVariable("ORDINI_DATABASE_CONNECTION")
            ?? throw new InvalidOperationException(
                "Variabile ORDINI_DATABASE_CONNECTION non configurata.");

        var repository = new ClienteRepository(connectionString);

        var email = $"integration.{Guid.NewGuid():N}@example.test";
        var cliente = new Cliente
        {
            Nome = "Cliente test integrazione",
            Email = email
        };
        var clienteCreato = false;

        try
        {
            var idCreato = await repository.AddAsync(cliente);
            clienteCreato = true;

            var clienteLetto = await repository.GetByEmailAsync(email);

            Assert.NotNull(clienteLetto);
            Assert.Equal(idCreato, clienteLetto.IdCliente);
            Assert.Equal(cliente.Nome, clienteLetto.Nome);
            Assert.Equal(email, clienteLetto.Email);
        }
        finally
        {
            if (clienteCreato)
            {
                await using var connection = new MySqlConnection(connectionString);

                await connection.ExecuteAsync(
                    "DELETE FROM Clienti WHERE Email = @Email;",
                    new { Email = email });
            }
        }
    }
}

internal static class TestEnvironment
{
    private const string EnvironmentVariableName = "ORDINI_DATABASE_CONNECTION";
    private const string LocalFileName = "appsettings.Development.json";

    public static void LoadFromLocalFile()
    {
        if (!string.IsNullOrWhiteSpace(
                Environment.GetEnvironmentVariable(EnvironmentVariableName)))
        {
            return;
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(LocalFileName, optional: false)
            .Build();

        var connectionString =
            configuration.GetConnectionString("OrdiniDatabase");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string 'OrdiniDatabase' non configurata in {LocalFileName}.");
        }

        Environment.SetEnvironmentVariable(
            EnvironmentVariableName,
            connectionString);
    }
}
