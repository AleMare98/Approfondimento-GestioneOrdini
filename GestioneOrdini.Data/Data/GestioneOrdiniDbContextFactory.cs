using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GestioneOrdini.Data.Data;

public class GestioneOrdiniDbContextFactory : IDesignTimeDbContextFactory<GestioneOrdiniDbContext>
{
    public GestioneOrdiniDbContext CreateDbContext(string[] args)
    {
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
        
        var options = new DbContextOptionsBuilder<GestioneOrdiniDbContext>()
            .UseMySQL(connectionString)
            .Options;

        return new GestioneOrdiniDbContext(options);
    }
}