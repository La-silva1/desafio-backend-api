using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using DesafioBackend.Data;

namespace DesafioBackend.Motos
{
    public class MotosService
    {
        private readonly AppDbContext _context;
        public MotosService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Moto?> CreateMoto(AddMotoRequest request)
        {
            var jaCadastrado = await _context.Motos
            .AnyAsync(moto => moto.Placa == request.Placa);

            if (jaCadastrado)
            {
                return null;
            }

            var newMoto = new Moto(request.Ano, request.Modelo, request.Placa);
            await _context.Motos.AddAsync(newMoto);
            await _context.SaveChangesAsync();

            if (request.Ano == 2024)
            {
                Console.WriteLine($" Moto 2024 cadastrada: {request.Modelo} - {request.Placa}");
            }

            return newMoto;
        }


        public async Task<IEnumerable<Moto>> GetMotos(string? placa)
        {
            var query = _context.Motos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(placa))
                query = query.Where(moto => moto.Placa.ToLower().Contains(placa.ToLower()));

            var motos = await query.ToListAsync();
            return motos;
        }

        public async Task<Moto?> UpDateMotos(Guid id, UpdateMotoRequest request)
        {
            var moto = await _context.Motos
            .SingleOrDefaultAsync(moto => moto.Id == id);

            if (moto == null)
            {
                return null;
            }

            moto.ModificarPlaca(request.Placa);
            await _context.SaveChangesAsync();

            return moto;
        }

        public async Task<Moto?> GetMotoById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return null;
            }

            var moto = await _context.Motos.SingleOrDefaultAsync(moto => moto.Id == id);

            return moto;
        }
        public async Task<DeleteResult> DeleteMoto(Guid id)
        {
            var moto = await _context.Motos
                .SingleOrDefaultAsync(moto => moto.Id == id);

            if (moto == null)
            {
                return DeleteResult.NotFound;
            }
            var locacoesAssociadas = await _context.Locacoes.AnyAsync(locacao => locacao.MotoId == moto.Id);

            if (locacoesAssociadas)
            {
                return DeleteResult.HasAssociatedRentals;
            }
            _context.Motos.Remove(moto);
            await _context.SaveChangesAsync();

            return DeleteResult.Success;

        }

    }
    public enum DeleteResult
    {
        Success,
        NotFound,
        HasAssociatedRentals
    }
}