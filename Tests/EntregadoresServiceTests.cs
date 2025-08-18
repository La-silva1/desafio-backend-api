using DesafioBackend.Data;
using DesafioBackend.Entregadores;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using System;

namespace DesafioBackend.Tests;

public class EntregadoresServiceTests
{
    private DbContextOptions<AppDbContext> CreateNewContextOptions(string dbName)
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
    }

    [Fact]
    public async Task CreateEntregador_ShouldSucceed_WhenDataIsValid()
    {
        var options = CreateNewContextOptions("TestDb_Entregador_CreateSuccess");
        var request = new AddEntregadorRequest
        {
            Nome = "João da Silva",
            Cnpj = "12345678901234",
            DataNascimento = new DateTime(1990, 1, 1),
            NumeroCNH = "12345678901",
            TipoCNH = "A",
            FotoCNH = ""
        };

        using (var context = new AppDbContext(options))
        {
            var service = new EntregadoresService(context);
            var (result, errorMessage) = await service.CreateEntregador(request);

            Assert.Null(errorMessage);
            Assert.NotNull(result);
            Assert.Equal(request.Nome, result.Nome);
        }
    }

    [Fact]
    public async Task CreateEntregador_ShouldFail_WhenCnhTypeIsInvalid()
    {
        var options = CreateNewContextOptions("TestDb_Entregador_InvalidCnhType");
        var request = new AddEntregadorRequest
        {
            Nome = "Maria Oliveira",
            Cnpj = "98765432109876",
            DataNascimento = new DateTime(1995, 5, 10),
            NumeroCNH = "98765432109",
            TipoCNH = "Z", 
            FotoCNH = ""
        };

        using (var context = new AppDbContext(options))
        {
            var service = new EntregadoresService(context);
            var (result, errorMessage) = await service.CreateEntregador(request);

            Assert.Null(result);
            Assert.NotNull(errorMessage);
            Assert.Equal("O tipo de CNH deve ser 'A', 'B' ou 'A+B'.", errorMessage);
        }
    }

    [Fact]
    public async Task CreateEntregador_ShouldFail_WhenCnhIsDuplicate()
    {
        var options = CreateNewContextOptions("TestDb_Entregador_DuplicateCnh");
        var existingEntregador = new Entregador("Carlos Pereira", "11223344556677", new DateTime(1988, 3, 15), "11223344556", "B", "");
        
        using (var context = new AppDbContext(options))
        {
            context.Entregadores.Add(existingEntregador);
            await context.SaveChangesAsync();
        }

        var request = new AddEntregadorRequest
        {
            Nome = "Ana Costa",
            Cnpj = "22334455667788",
            DataNascimento = new DateTime(1992, 7, 20),
            NumeroCNH = "11223344556", 
            TipoCNH = "A",
            FotoCNH = ""
        };

        using (var context = new AppDbContext(options))
        {
            var service = new EntregadoresService(context);
            var (result, errorMessage) = await service.CreateEntregador(request);

            Assert.Null(result);
            Assert.NotNull(errorMessage);
            Assert.Equal("CNH já cadastrada.", errorMessage);
        }
    }
}
