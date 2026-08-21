using Dapper;
using System.Data.Common;
using GestioneOrdini.Data.Data;
using GestioneOrdini.Data.Models;
using GestioneOrdini.Data.Repositories;
using Microsoft.EntityFrameworkCore;
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

    [Fact]
    public async Task AddAsync_then_GetByIdAsync_returns_order_with_customer_lines_and_products()
    {
        TestEnvironment.LoadFromLocalFile();

        var connectionString =
            Environment.GetEnvironmentVariable("ORDINI_DATABASE_CONNECTION")
            ?? throw new InvalidOperationException(
                "Variabile ORDINI_DATABASE_CONNECTION non configurata.");

        var idClienteCreato = 0;
        var idPrimoProdottoCreato = 0;
        var idSecondoProdottoCreato = 0;
        var idOrdineCreato = 0;

        try
        {
            var clienteRepository = new ClienteRepository(connectionString);
            var prodottoRepository = new ProdottoRepository(connectionString);

            var emailCliente = $"ordine.ef.{Guid.NewGuid():N}@example.test";
            var nomePrimoProdotto = $"Prodotto EF A {Guid.NewGuid():N}";
            var nomeSecondoProdotto = $"Prodotto EF B {Guid.NewGuid():N}";

            idClienteCreato = await clienteRepository.AddAsync(new Cliente
            {
                Nome = "Cliente test ordine EF",
                Email = emailCliente
            });

            idPrimoProdottoCreato = await prodottoRepository.AddAsync(new Prodotto
            {
                Nome = nomePrimoProdotto,
                Prezzo = 10.00m
            });

            idSecondoProdottoCreato = await prodottoRepository.AddAsync(new Prodotto
            {
                Nome = nomeSecondoProdotto,
                Prezzo = 20.00m
            });

            var efConnectionStringBuilder = new DbConnectionStringBuilder
            {
                ConnectionString = connectionString
            };
            efConnectionStringBuilder["SslMode"] = "Disabled";

            var options = new DbContextOptionsBuilder<GestioneOrdiniDbContext>()
                .UseMySQL(efConnectionStringBuilder.ConnectionString)
                .Options;

            await using var dbContext = new GestioneOrdiniDbContext(options);
            var repository = new OrdineEfRepository(dbContext);

            idOrdineCreato = await repository.AddAsync(new NuovoOrdine(
                idClienteCreato,
                [
                    new NuovaRigaOrdine(idPrimoProdottoCreato, 1, 10.00m),
                    new NuovaRigaOrdine(idSecondoProdottoCreato, 2, 20.00m)
                ]));

            var ordineLetto = await repository.GetByIdAsync(idOrdineCreato);

            Assert.NotNull(ordineLetto);
            Assert.Equal(idOrdineCreato, ordineLetto.IdOrdine);
            Assert.Equal(idClienteCreato, ordineLetto.IdCliente);
            Assert.Equal(emailCliente, ordineLetto.Cliente.Email);
            Assert.Equal(2, ordineLetto.Righe.Count);

            var primaRiga = ordineLetto.Righe.Single(
                riga => riga.IdProdotto == idPrimoProdottoCreato);
            var secondaRiga = ordineLetto.Righe.Single(
                riga => riga.IdProdotto == idSecondoProdottoCreato);

            Assert.Equal(1, primaRiga.Quantita);
            Assert.Equal(nomePrimoProdotto, primaRiga.Prodotto.Nome);
            Assert.Equal(2, secondaRiga.Quantita);
            Assert.Equal(nomeSecondoProdotto, secondaRiga.Prodotto.Nome);
        }
        finally
        {
            await using var connection = new MySqlConnection(connectionString);

            if (idOrdineCreato > 0)
            {
                await connection.ExecuteAsync(
                    "DELETE FROM RigheOrdine WHERE IdOrdine = @IdOrdine; " +
                    "DELETE FROM Ordini WHERE IdOrdine = @IdOrdine;",
                    new { IdOrdine = idOrdineCreato });
            }

            var idProdottiCreati = new[]
                { idPrimoProdottoCreato, idSecondoProdottoCreato }
                .Where(idProdotto => idProdotto > 0)
                .ToArray();

            if (idProdottiCreati.Length > 0)
            {
                await connection.ExecuteAsync(
                    "DELETE FROM Prodotti WHERE IdProdotto IN @IdProdotto;",
                    new { IdProdotto = idProdottiCreati });
            }

            if (idClienteCreato > 0)
            {
                await connection.ExecuteAsync(
                    "DELETE FROM Clienti WHERE IdCliente = @IdCliente;",
                    new { IdCliente = idClienteCreato });
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
