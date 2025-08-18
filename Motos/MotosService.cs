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

        public async Task<(Moto? Moto, string? ErrorMessage)> CreateMoto(AddMotoRequest request)
        {
            var placaJaCadastrada = await _context.Motos
                .AnyAsync(moto => moto.Placa == request.Placa);

            if (placaJaCadastrada)
            {
                return (null, "Placa já cadastrada.");
            }

            var newMoto = new Moto(request.Ano, request.Modelo, request.Placa);
            await _context.Motos.AddAsync(newMoto);
            await _context.SaveChangesAsync();

            return (newMoto, null);
        }


        public async Task<IEnumerable<Moto>> GetMotos(string? placa)
        {
            var query = _context.Motos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(placa))
                query = query.Where(moto => moto.Placa.ToLower().Contains(placa.ToLower()));

            var motos = await query.ToListAsync();
            return motos;
        }

        public async Task<(Moto? Moto, string? ErrorMessage)> UpDateMotos(Guid id, UpdateMotoRequest request)
        {
            var moto = await _context.Motos
                .SingleOrDefaultAsync(moto => moto.Id == id);

            if (moto == null)
            {
                return (null, "Moto não encontrada!");
            }

            var placaJaCadastrada = await _context.Motos
            .AnyAsync(moto => moto.Placa == request.Placa && moto.Id != id);

            if (placaJaCadastrada)
            {
                return (null, "Placa já cadastrada para outra moto.");
            }

            moto.ModificarPlaca(request.Placa);
                await _context.SaveChangesAsync();

            return (moto, null);
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
        public async Task<(int? Codigo, string? ErrorMessage)> DeleteMoto(Guid id)
        {
            var moto = await _context.Motos
                .SingleOrDefaultAsync(moto => moto.Id == id);

            if (moto == null)
            {
                 return (404, "Moto não encontrada!");
            }
            var locacoesAssociadas = await _context.Locacoes
            .AnyAsync(locacao => locacao.MotoId == moto.Id);

            if (locacoesAssociadas)
            {
                return (400, "Esta moto não pode ser excluída pois possui locações associadas.");           
            }

            _context.Motos.Remove(moto);
            await _context.SaveChangesAsync();

            return (200, null);

        }

    }

}