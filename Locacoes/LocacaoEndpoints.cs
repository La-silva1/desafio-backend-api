using DesafioBackend.Locacoes;
using DesafioBackend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                var (newLocacao, errorMessage) = await service.CreateLocacao(request);

                if (newLocacao == null)
                { 
                    return Results.BadRequest(new { mensagem = errorMessage });
                }

                return Results.Created($"/locacao/{newLocacao.EntregadorId}", newLocacao);
            })
        .WithSummary("Alugar uma moto");

        endpointsLocacoes.MapGet("/{id}",
        async ([FromRoute] string id, AppDbContext context) =>
        {
            var service = new LocacoesService(context);
            var (locacao, errorMessage) = await service.GetLocacaoById(id);

            if (locacao == null)
            {
                return Results.NotFound(new { mensagem = errorMessage });
            }

            return Results.Ok(locacao);
        })
        .WithSummary("Consultar locação pelo ID");


        endpointsLocacoes.MapPut("/{id}/devolucao", 
            async ([FromRoute] string id, [FromBody] DevolverMotoRequest request, AppDbContext context) =>
            {
                var service = new LocacoesService(context);
                var (custoTotal, errorMessage) = await service.DevolverMoto(id, request);

                if (custoTotal == null)
                {
                     if (errorMessage!.Contains("Locação não encontrada"))
                    {
                        return Results.NotFound(new { mensagem = errorMessage });
                    }
                    else
                    {
                        return Results.BadRequest(new { mensagem = errorMessage });
                    }
                }

                return Results.Ok(new { CustoTotal = custoTotal });
            })
        .WithSummary("Informar data de devolução e calcular valor");
    }
}
