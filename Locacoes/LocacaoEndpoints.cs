using DesafioBackend.Locacoes;
using DesafioBackend.Data;
using DesafioBackend.Entregadores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http; 

namespace DesafioBackend.Locacoes;

public static class LocacoesEndpoints
{

    public static void AddEndpointsLocacoes(this WebApplication app)
    {
        var endpointsLocacoes = app.MapGroup(prefix: "locacao").WithTags("Locação");

        endpointsLocacoes.MapPost("",
            async ([FromBody] AddLocacaoRequest request, AppDbContext context) =>
            {
                var service = new LocacoesService(context);
                var resultado = await service.CreateLocacaoAsync(request);

                if (!resultado.IsSuccess)
                {
                    return Results.BadRequest(new { mensagem = resultado.ErrorMessage });
                }

                return Results.Created($"/locacao/{resultado.Locacao!.EntregadorId}", resultado.Locacao);
            })
        .WithSummary("Alugar uma moto");

        endpointsLocacoes.MapGet("/{id}",
        async ([FromRoute] string id, AppDbContext context) =>
        {
            var service = new LocacoesService(context);
            var locacaoDto = await service.GetLocacaoByIdAsync(id);

            if (locacaoDto == null)
            {
                return Results.NotFound("Locação não encontrada");
            }

            return Results.Ok(locacaoDto);
        })
        .WithSummary("Consultar locação pelo ID");


        endpointsLocacoes.MapPut("/{id}/devolucao", 
            async ([FromRoute] string id, [FromBody] DevolverMotoRequest request, AppDbContext context) =>
            {
                var service = new LocacoesService(context);
                var resultado = await service.DevolverMotoAsync(id, request);

                if (resultado.Status == LocacoesService.DevolucaoStatus.NotFound)
                {
                    return Results.NotFound(resultado.ErrorMessage);
                }
                else if (resultado.Status == LocacoesService.DevolucaoStatus.InvalidPlan)
                {
                    return Results.BadRequest(resultado.ErrorMessage);
                }

                return Results.Ok(new { CustoTotal = resultado.CustoTotal });
            })
        .WithSummary("Informar data de devolução e calcular valor");
    }
}
