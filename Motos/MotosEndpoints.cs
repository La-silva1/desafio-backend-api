using DesafioBackend.Motos;
using DesafioBackend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace DesafioBackend.Motos;

public static class MotosEndpoints
{
    public static void AddEndpointsMotos(this WebApplication app)
    {
        var endpointsMotos = app.MapGroup(prefix: "motos").WithTags("Motos");
        
        endpointsMotos.MapPost("",
            async ([FromBody] AddMotoRequest request, AppDbContext context) =>
        {
            var service = new MotosService(context);
            var (newMoto, errorMessage) = await service.CreateMoto(request);

            if (newMoto != null)
            {
                return Results.Created($"/motos/{newMoto.Id}", newMoto);
            }
            if (errorMessage!.Contains("já cadastrada", StringComparison.OrdinalIgnoreCase) ||
                errorMessage!.Contains("já cadastrado", StringComparison.OrdinalIgnoreCase))
            {
                return Results.Conflict(new { mensagem = errorMessage });
            }
        
            return Results.BadRequest(new { mensagem = errorMessage });
        })
        .WithSummary("Cadastrar uma nova moto");


        endpointsMotos.MapGet("",
            async ([FromQuery] string? placa, AppDbContext context) =>
        {
            var service = new MotosService(context);
            var motos = await service.GetMotos(placa);

            return Results.Ok(motos);
        })
        .WithSummary("Consultar motos existentes");

        endpointsMotos.MapPut("{id}/placa",
            async (Guid id, UpdateMotoRequest request, AppDbContext context) =>
            {
                var service = new MotosService(context);
                var (motoAtualizada, errorMessage) = await service.UpDateMotos(id, request);
                if (motoAtualizada == null)
                {
                    return Results.BadRequest(new { mensagem = errorMessage });
                }
                return Results.Ok(new { mensagem = "placa modificada com sucesso", moto = motoAtualizada });
            })
             .WithSummary("Modificar a placa de uma moto");

        endpointsMotos.MapGet("{id:Guid}",
            async (Guid id, AppDbContext context) =>
            {
                var service = new MotosService(context);
                var moto = await service.GetMotoById(id);
                if (moto == null)
                {
                    return Results.NotFound(new { mensagem = "moto não encontrada" });
                }
                return Results.Ok(moto);
            })
            .WithSummary("Consultar motos existentes por Id");

        endpointsMotos.MapDelete("{id}",
        async (Guid id, AppDbContext context) =>
        {
            var service = new MotosService(context);
            var (codigo, errorMessage) = await service.DeleteMoto(id);
            if (codigo == 404)
            {
                return Results.NotFound(new { mensagem = errorMessage });
            }
            else if (codigo == 400)
            {
                return Results.BadRequest(new { mensagem = errorMessage });
            }
            return Results.Ok();
        })
                .WithSummary("Remover uma moto");
    }

}
