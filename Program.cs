using DesafioBackend.Motos;
using DesafioBackend.Data;
using DesafioBackend.Entregadores;
using DesafioBackend.Locacoes;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAntiforgery();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Sistema de Manutenção de Motos",
        Version = "v1"
    });

});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddOpenApi();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

// Necessario para migração, e fazer retry até que o db esteja disponivel
var maxRetries = 5;
for (int i = 1; i <= maxRetries; i++)
{
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
            logger.LogInformation("Migração realizada com sucesso.");
            break;
        }
    }
    catch (Npgsql.NpgsqlException ex) when (ex.InnerException is System.Net.Sockets.SocketException)
    {
        logger.LogWarning(ex, "Conexão com database falhou. Tentativa {Attempt} de {MaxRetries}. Refazendo em 5 segundos...", i, maxRetries);
        if (i == maxRetries) throw;
        System.Threading.Thread.Sleep(5000);
    }
}


app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAntiforgery();


//Configurando as rotas 
app.AddEndpointsMotos();
app.AddEndpointsEntregadores();
app.AddEndpointsLocacoes();

app.Run();

