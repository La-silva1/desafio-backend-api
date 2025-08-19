using DesafioBackend.Data;
using DesafioBackend.Locacoes;
using DesafioBackend.Entregadores; 
using DesafioBackend.Motos; 
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace DesafioBackend.Tests;

public class LocacoesServiceTests
{
    private DbContextOptions<AppDbContext> CreateNewContextOptions(string dbName)
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
    }

    [Fact]
    public async Task CreateLocacao_ShouldSucceed_WhenDataIsValid()
    {
        var options = CreateNewContextOptions("TestDb_Locacao_CreateSuccess");

        var entregadorId = Guid.NewGuid(); 
        var motoId = Guid.NewGuid();     

        var mockEntregador = new Entregador(
            "Entregador Teste",
            "12345678901234",
            new DateTime(1990, 1, 1),
            "12345678901",
            "A", 
            ""
        ) { Id = entregadorId }; 

        var mockMoto = new Moto( 
            2020,
            "Modelo Teste",
            "ABC-1234"
        ) { Id = motoId }; 

        using (var context = new AppDbContext(options))
        {
            context.Entregadores.Add(mockEntregador);
            context.Motos.Add(mockMoto);
            await context.SaveChangesAsync();
        }

        var planos = new Dictionary<string, (int duracaoDias, decimal custoDiario)>
        {
            { "7 dias", (7, 30.00m) },
            { "15 dias", (15, 28.00m) },
            { "30 dias", (30, 22.00m) },
            { "45 dias", (45, 20.00m) },
            { "50 dias", (50, 18.00m) }
        };

        var planoEscolhido = "7 dias";
        var (duracaoDias, _) = planos[planoEscolhido];
        var dataInicioLocacao = DateTime.Today.AddDays(1);
        var dataPrevisaoTermino = dataInicioLocacao.AddDays(duracaoDias);

        var request = new AddLocacaoRequest
        {
            EntregadorId = entregadorId,
            MotoId = motoId,
            Plano = planoEscolhido,
            DataInicio = dataInicioLocacao, 
            DataTermino = dataPrevisaoTermino, 
            DataPrevisaoTermino = dataPrevisaoTermino 
        };

        using (var context = new AppDbContext(options))
        {
            var service = new LocacoesService(context);
            var (result, errorMessage) = await service.CreateLocacao(request);

            Assert.Null(errorMessage);
            Assert.NotNull(result);
            Assert.Equal(request.EntregadorId, result.EntregadorId);
            Assert.Equal(request.MotoId, result.MotoId);
            Assert.Equal(request.Plano, result.Plano);
            Assert.True(result.DataInicio > DateTime.MinValue); 
            Assert.True(result.DataPrevisaoTermino > DateTime.MinValue); 
        }
    }
}