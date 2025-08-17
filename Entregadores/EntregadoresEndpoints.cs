using DesafioBackend.Motos;
using DesafioBackend.Data;
using DesafioBackend.Entregadores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace DesafioBackend.Entregadores;

public record EntregadorRequest(string Nome, string Cnpj, DateTime DataNascimento,
string NumeroCNH, string TipoCNH, string FotoCNH);

public static class EntregadoresEndpoints
{
    public static void AddEndpointsEntregadores(this WebApplication app)
    {
        var endpointsEntregadores = app.MapGroup(prefix: "entregadores").WithTags("Entregadores");

        endpointsEntregadores.MapPost("",
            async ([FromBody] AddEntregadorRequest request, AppDbContext context) =>
        {
            var service = new EntregadoresService(context);
            var (newEntregador, errorMessage) = await service.CreateEntregador(request);

            if (newEntregador != null)
            {
                return Results.Created($"/entregadores/{newEntregador.Id}", newEntregador);
            }
            if (errorMessage!.Contains("já cadastrado", StringComparison.OrdinalIgnoreCase))
            {
                return Results.Conflict(errorMessage);
            }
        
            return Results.BadRequest(errorMessage);
        })
        .WithSummary("Cadastrar um novo Entregador");

        endpointsEntregadores.MapPost("{id}/cnh",
            async (Guid id, AddEntregadorCnh request, AppDbContext context) =>
            {
                var service = new EntregadoresService(context);
                var (codigo, errorMessage) = await service.UploadCnh(id, request);
                
                if (codigo == 404)
                {
                    return Results.NotFound(new { mensagem = errorMessage });
                }
                else if (codigo == 400)
                {
                    return Results.BadRequest(new { mensagem = errorMessage });
                }

                return Results.Ok(new { mensagem = "CNH salva com sucesso." });
        })
        .WithSummary("Enviar foto CNH");
    }
}



