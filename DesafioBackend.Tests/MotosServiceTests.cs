using DesafioBackend.Data;
using DesafioBackend.Motos;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;

namespace DesafioBackend.Tests;

public class MotosServiceTests
{
    private DbContextOptions<AppDbContext> CreateNewContextOptions(string dbName)
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
    }

    [Fact]
    public async Task CreateMoto_ShouldAddNewMoto_WhenPlacaIsUnique()
    {
        // Arrange
        var options = CreateNewContextOptions("TestDb_CreateSuccess");
        var request = new AddMotoRequest("Honda Titan", "ABC1234", 2022);

        // Act
        using (var context = new AppDbContext(options))
        {
            var service = new MotosService(context);
            var (result, errorMessage) = await service.CreateMoto(request);
            
            // Assert
            Assert.Null(errorMessage);
            Assert.NotNull(result);
            Assert.Equal(request.Ano, result.Ano);
        }

        // Assert - verify in a separate context to ensure it was saved
        using (var context = new AppDbContext(options))
        {
            var motoInDb = await context.Motos.SingleOrDefaultAsync(m => m.Placa == "ABC1234");
            Assert.NotNull(motoInDb);
            Assert.Equal(2022, motoInDb.Ano);
        }
    }

    [Fact]
    public async Task CreateMoto_ShouldReturnError_WhenPlacaExists()
    {
        // Arrange
        var options = CreateNewContextOptions("TestDb_CreateFailure");
        var existingMoto = new Moto(2020, "Yamaha Fazer", "XYZ5678");

        // Pre-seed the database
        using (var context = new AppDbContext(options))
        {
            context.Motos.Add(existingMoto);
            await context.SaveChangesAsync();
        }

        var request = new AddMotoRequest("Honda Biz", "XYZ5678", 2022);

        // Act & Assert
        using (var context = new AppDbContext(options))
        {
            var service = new MotosService(context);
            var (result, errorMessage) = await service.CreateMoto(request);

            Assert.Null(result);
            Assert.NotNull(errorMessage);
            Assert.Equal("Placa já cadastrada.", errorMessage);
        }
    }
}
