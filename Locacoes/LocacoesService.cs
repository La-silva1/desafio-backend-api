using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DesafioBackend.Data;

namespace DesafioBackend.Locacoes
{
    public class LocacoesService
    {
        private readonly AppDbContext _context;
        private static readonly Dictionary<string, (int duracaoDias, decimal custoDiario)> Planos =
            new Dictionary<string, (int duracaoDias, decimal custoDiario)>
            {
                { "7 dias", (7, 30.00m) },
                { "15 dias", (15, 28.00m) },
                { "30 dias", (30, 22.00m) },
                { "45 dias", (45, 20.00m) },
                { "50 dias", (50, 18.00m) }
            };

        private static readonly Dictionary<string, decimal> CustosMulta =
            new Dictionary<string, decimal>
            {
                { "7 dias", 50.00m },
                { "15 dias", 50.00m },
                { "30 dias", 50.00m },
                { "45 dias", 50.00m },
                { "50 dias", 50.00m }
            };

        public LocacoesService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(Locacao? Locacao, string? ErrorMessage)> CreateLocacao(AddLocacaoRequest request)
        {
            var entregador = await _context.Entregadores.FindAsync(request.EntregadorId);
            if (entregador == null)
            {
                return (null, $"Entregador com ID '{request.EntregadorId}' não encontrado.");
            }

            var moto = await _context.Motos.FindAsync(request.MotoId);
            if (moto == null)
            {
                return (null, $"Moto com ID '{request.MotoId}' não encontrada.");
            }

            if (!entregador.TipoCNH.Contains("A"))
            {
                return (null, "Entregador não habilitado para locação de motos. Apenas entregadores com CNH categoria 'A' podem alugar.");
            }

            if (string.IsNullOrEmpty(request.Plano) || !Planos.ContainsKey(request.Plano))
            {
                return (null, "Plano de locação inválido. Esolha entre 7, 15, 30, 45 e 50 dias");
            }

            var (duracaoDias, custoDiario) = Planos[request.Plano];
            var dataInicioLocacao = DateTime.Today.AddDays(1);
            var dataPrevisaoTermino = dataInicioLocacao.AddDays(duracaoDias);

            var newLocacao = new Locacao(
                entregador.Id,
                moto.Id,
                dataInicioLocacao.ToUniversalTime(),
                dataPrevisaoTermino.ToUniversalTime(),
                dataPrevisaoTermino.ToUniversalTime(),
                request.Plano
            );

            await _context.Locacoes.AddAsync(newLocacao);
            await _context.SaveChangesAsync();

            return (newLocacao, null);
        }

        public async Task<(Locacao? Locacao, string? ErrorMessage)> GetLocacaoById(string id)
        {
            var locacao = await _context.Locacoes.FirstOrDefaultAsync(l => l.Id == id);

            if (locacao == null)
            {
                return (null, "Locação não encontrada.");
            }

            if (string.IsNullOrEmpty(locacao.Plano) || !Planos.ContainsKey(locacao.Plano))
            {
                return (null, "Plano de locação inválido ou não encontrado.");
            }

            return (locacao, null);
        }

        public async Task<(decimal? CustoTotal, string? ErrorMessage)> DevolverMoto(string id, DevolverMotoRequest request)
        {
            var locacao = await _context.Locacoes.FirstOrDefaultAsync(l => l.Id == id);

            if (locacao == null)
            {
                return (null, "Locação não encontrada.");
            }

            if (string.IsNullOrEmpty(locacao.Plano) || !Planos.ContainsKey(locacao.Plano))
            {
                return (null, "Plano de locação inválido.");
            }

            var (duracaoDias, custoDiario) = Planos[locacao.Plano];
            var custoMultaDiaria = CustosMulta[locacao.Plano];

            decimal custoTotal = 0;
            decimal multaAdicional = 0;

            var duracaoReal = (request.DataTermino - locacao.DataInicio).TotalDays;

            if (request.DataTermino < locacao.DataPrevisaoTermino)
            {
                multaAdicional = GetValorMultaAdicional(locacao.DataPrevisaoTermino, request.DataTermino, custoDiario, locacao.Plano);
                custoTotal = ((decimal)duracaoReal * custoDiario) + multaAdicional;
            }
            else
            {
                var diasAtraso = (request.DataTermino - locacao.DataPrevisaoTermino).TotalDays;
                custoTotal = (duracaoDias * custoDiario) + ((decimal)diasAtraso * custoMultaDiaria);
            }

            await _context.SaveChangesAsync();

            return (custoTotal, null);
        }

        private decimal GetValorMultaAdicional(DateTime previsaoTermino, DateTime dataTermino, decimal custoDiario, string plano)
        {
            var diasNaoEfetivados = (previsaoTermino - dataTermino).TotalDays;
            decimal valorDiariasNaoEfetivadas = (decimal)diasNaoEfetivados * custoDiario;
            decimal multaAdicional = 0;

            if (plano == "7 dias")
            {
                multaAdicional = valorDiariasNaoEfetivadas * 0.20m;
            }
            else if (plano == "15 dias")
            {
                multaAdicional = valorDiariasNaoEfetivadas * 0.40m;
            }
            else
            {
                multaAdicional = valorDiariasNaoEfetivadas * 0.20m;
            }

            return multaAdicional;
        }
    }
}
