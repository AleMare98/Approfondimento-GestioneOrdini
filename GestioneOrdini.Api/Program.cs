using GestioneOrdini.Api.Middleware;
using GestioneOrdini.Api.Services;
using GestioneOrdini.Data.Repositories;
using GestioneOrdini.Data.Data;
using Microsoft.EntityFrameworkCore;

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


// EF
builder.Services.AddDbContext<GestioneOrdiniDbContext>(options => options.UseMySQL(connectionString));

builder.Services.AddScoped<IOrdineEfRepository, OrdineEfRepository>();
builder.Services.AddScoped<OrdineService>();

var app = builder.Build();

// Error handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Correlation ID middleware
app.UseMiddleware<CorrelationIdMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
