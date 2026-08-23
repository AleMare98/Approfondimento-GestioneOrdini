using GestioneOrdini.Api.Services;
using GestioneOrdini.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration
                           .GetConnectionString("OrdiniDatabase")
                       ?? throw new InvalidOperationException(
                           "Connection string 'OrdiniDatabase' non configurata.");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IClienteRepository>(
    _ => new ClienteRepository(connectionString));

builder.Services.AddScoped<ClienteService>();

builder.Services.AddScoped<IProdottoRepository>(_ => new ProdottoRepository(connectionString));
builder.Services.AddScoped<ProdottoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();