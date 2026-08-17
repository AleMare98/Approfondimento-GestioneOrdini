using GestioneOrdini.Data.Models;
using GestioneOrdini.Data.Repositories;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

const string connectionStringName = "OrdiniDatabase";

var environment =
    Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .Build();

var connectionString = configuration.GetConnectionString(connectionStringName);

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        $"Connection string '{connectionStringName}' non configurata.");
}

await using var connection = new MySqlConnection(connectionString);
await connection.OpenAsync();

Console.WriteLine("Connessione a MySQL riuscita.");

var repository = new ClienteRepository(connectionString);

var nuovoCliente = new Cliente
{
    Nome = "Grace Hopper",
    Email = $"grace.hopper.{Guid.NewGuid():N}@example.test"
};

var idClienteCreato = await repository.AddAsync(nuovoCliente);

var clienteCreato = await repository.GetByIdAsync(idClienteCreato);

Console.WriteLine(
    clienteCreato is null
        ? "Errore: cliente appena creato non trovato."
        : $"Creato: {clienteCreato.IdCliente} - {clienteCreato.Nome}");
        
var prodottoRepository = new ProdottoRepository(connectionString);

var nuovoProdotto = new Prodotto
{
    Nome = "Manuale C#",
    Prezzo = 29.90m
};

var idProdottoCreato = await prodottoRepository.AddAsync(nuovoProdotto);

var prodottoCreato = await prodottoRepository.GetByIdAsync(idProdottoCreato);

Console.WriteLine(
    prodottoCreato is null
        ? "Errore: prodotto appena creato non trovato."
        : $"Creato: {prodottoCreato.IdProdotto} - {prodottoCreato.Nome} - {prodottoCreato.Prezzo:C}");
        
        
var ordineRepository = new OrdineRepository(connectionString);

var ordineConErrore = new NuovoOrdine(
    IdCliente: 1,
    Righe:
    [
        new NuovaRigaOrdine(
            IdProdotto: 1,
            Quantita: 1,
            PrezzoUnitario: 29.90m),

        new NuovaRigaOrdine(
            IdProdotto: 999999,
            Quantita: 1,
            PrezzoUnitario: 10.00m)
    ]);

try
{
    await ordineRepository.AddAsync(ordineConErrore);

    Console.WriteLine("Errore: l'ordine non avrebbe dovuto riuscire.");
}
catch (MySqlException exception)
{
    Console.WriteLine(
        $"Inserimento fallito come previsto. Errore MySQL: {exception.Number}");
}