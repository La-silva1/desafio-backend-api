using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using DesafioBackend.Data;
using DesafioBackend.Entregadores;
using System.Threading.Tasks;
using System.IO;

namespace DesafioBackend.Entregadores
{
    public class EntregadoresService
    {
        private readonly AppDbContext _context;
        public EntregadoresService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<(Entregador? Entregador, string? ErrorMessage)> CreateEntregador(AddEntregadorRequest request)
        {
            string tipoCNHUpper = request.TipoCNH?.ToUpper() ?? string.Empty;
            if (tipoCNHUpper != "A" && tipoCNHUpper != "B" && tipoCNHUpper != "A+B")
            {
                return (null, "O tipo de CNH deve ser 'A', 'B' ou 'A+B'.");
            }
            var cnpjPattern = @"^\d{14}$";
            if (!Regex.IsMatch(request.Cnpj, cnpjPattern))
            {
                return (null, "O CNPJ deve ter 14 dígitos numéricos.");
            }

            var cnhPattern = @"^\d{11}$";
            if (!Regex.IsMatch(request.NumeroCNH, cnhPattern))
            {
                return (null, "O número da CNH deve ter 11 dígitos numéricos.");
            }

            if (request.DataNascimento.Date > DateTime.Now.Date.AddYears(-18))
            {
                return (null, "A data de nascimento não é válida. O entregador deve ter no mínimo 18 anos.");
            }
            var cnhJaCadastrada = await _context.Entregadores
           .AnyAsync(entregador => entregador.NumeroCNH == request.NumeroCNH);

            if (cnhJaCadastrada)
            {
                return (null, "CNH já cadastrada.");
            }

            var cnpjJaCadastrado = await _context.Entregadores
            .AnyAsync(entregador => entregador.Cnpj == request.Cnpj);

            if (cnpjJaCadastrado)
            {
                return (null, "Cnpj já cadastrado");
            }
            try
            {
                var arquivoPath = (string?)"";
                if (request.FotoCNH != "")
                {
                    arquivoPath = await SaveLocalPhoto(request.FotoCNH);
                }
                var newEntregador = new Entregador(
                    request.Nome,
                    request.Cnpj,
                    request.DataNascimento,
                    request.NumeroCNH,
                    request.TipoCNH!,
                    arquivoPath
                );
                await _context.Entregadores.AddAsync(newEntregador);
                await _context.SaveChangesAsync();

                return (newEntregador, null);
            }
            catch (FormatException)
            {
                return (null, "Formato de Base64 Invalido!");
            }
        }

        public async Task<(int? Codigo, string? ErrorMessage)> UploadCnh(Guid id, AddEntregadorCnh request)
        {
            var entregador = await _context.Entregadores
                .SingleOrDefaultAsync(entregador => entregador.Id == id);

            if (entregador == null)
            {
                return (404, "Entregador não encontrado!");
            }
            var diretorioUploads = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "CNH");

            if (!Directory.Exists(diretorioUploads))
            {
                Directory.CreateDirectory(diretorioUploads);
            }

            try
            {
                var arquivoPath = await SaveLocalPhoto(request.ImagemCnh);
                entregador.ModificarFotoCnh(arquivoPath);
                await _context.SaveChangesAsync();
                return (200, null);
            }
            catch (FormatException)
            {
                return (400, "Formato de Base64 Invalido!");
            }
        }

        private async Task<string> SaveLocalPhoto(string imagem)
        {
            var diretorioUploads = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "CNH");

            if (!Directory.Exists(diretorioUploads))
            {
                Directory.CreateDirectory(diretorioUploads);
            }

            if (imagem.Contains(","))
            {
                string[] partes = imagem.Split(',');
                imagem = partes[1];
            }
            byte[] bytesArquivo = Convert.FromBase64String(imagem);
            var nomeArquivo = $"{Guid.NewGuid()}.png";
            var caminhoArquivo = Path.Combine(diretorioUploads, nomeArquivo);

            await File.WriteAllBytesAsync(caminhoArquivo, bytesArquivo);

            return caminhoArquivo;
        }
    }
}