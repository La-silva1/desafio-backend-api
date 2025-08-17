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
            var newMoto = await service.CreateMoto(request);

            if (newMoto == null)
            {
                return Results.Conflict("Moto já cadastrada");
            }

            return Results.Created($"/motos/{newMoto.Id}", newMoto);
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
                var motoAtualizada = await service.UpDateMotos(id, request);
                
                if (motoAtualizada == null)
                {
                    return Results.BadRequest(new { mensagem = "dados inválidos" });
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
            var resultado = await service.DeleteMoto(id);
            if (resultado == DeleteResult.NotFound)
            {
                return Results.NotFound(new { mensagem = "Moto não encontrada." });
            }

            else if (resultado == DeleteResult.HasAssociatedRentals)
            {
                return Results.BadRequest(new { mensagem = "Moto com locações associadas, não pode ser removida." });
            }
            return Results.Ok();
        })
                .WithSummary("Remover uma moto");
    }

}
