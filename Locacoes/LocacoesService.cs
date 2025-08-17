using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DesafioBackend.Data;
using DesafioBackend.Locacoes;

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

    public class LocacaoCreationResult
    {
        public Locacao? Locacao { get; set; }
        public string? ErrorMessage { get; set; }
        public bool IsSuccess => Locacao != null && ErrorMessage == null;
    }

    public class LocacaoResponseDto
    {
        public string? IdLocacao { get; set; }
        public decimal ValorDiaria { get; set; }
        public Guid IdEntregador { get; set; }
        public Guid IdMoto { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataTermino { get; set; }
        public DateTime DataPrevisaoDevolucao { get; set; }
    }

    public class DevolucaoResult
    {
        public DevolucaoStatus Status { get; set; }
        public decimal CustoTotal { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public enum DevolucaoStatus
    {
        Success,
        NotFound,
        InvalidPlan
    }

    public async Task<LocacaoCreationResult> CreateLocacaoAsync(AddLocacaoRequest request)
    {
        var entregador = await _context.Entregadores.FindAsync(request.EntregadorId);
        if (entregador == null)
        {
            return new LocacaoCreationResult { ErrorMessage = $"Entregador com ID '{request.EntregadorId}' não encontrado." };
        }

        var moto = await _context.Motos.FindAsync(request.MotoId);
        if (moto == null)
        {
            return new LocacaoCreationResult { ErrorMessage = $"Moto com ID '{request.MotoId}' não encontrada." };
        }

        if (!entregador.TipoCNH.Contains("A"))
        {
            return new LocacaoCreationResult { ErrorMessage = "Entregador não habilitado para locação de motos. Apenas entregadores com CNH categoria 'A' podem alugar." };
        }

        if (string.IsNullOrEmpty(request.Plano) || !Planos.ContainsKey(request.Plano))
        {
            return new LocacaoCreationResult { ErrorMessage = "Plano de locação inválido." };
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

        return new LocacaoCreationResult { Locacao = newLocacao };
    }

    public async Task<LocacaoResponseDto?> GetLocacaoByIdAsync(string id)
    {
        var locacao = await _context.Locacoes.FirstOrDefaultAsync(l => l.Id == id);

        if (locacao == null)
        {
            return null;
        }

        if (string.IsNullOrEmpty(locacao.Plano) || !Planos.ContainsKey(locacao.Plano))
        {
            return null;
        }

        var (duracaoDias, custoDiario) = Planos[locacao.Plano];

        return new LocacaoResponseDto
        {
            IdLocacao = locacao.Id,
            ValorDiaria = custoDiario,
            IdEntregador = locacao.EntregadorId,
            IdMoto = locacao.MotoId,
            DataInicio = locacao.DataInicio,
            DataTermino = locacao.DataTermino,
            DataPrevisaoDevolucao = locacao.DataPrevisaoTermino
        };
    }

    public async Task<DevolucaoResult> DevolverMotoAsync(string id, DevolverMotoRequest request)
    {
        var locacao = await _context.Locacoes.FirstOrDefaultAsync(l => l.Id == id);

        if (locacao == null)
        {
            return new DevolucaoResult { Status = DevolucaoStatus.NotFound, ErrorMessage = "Locação não encontrada." };
        }

        if (string.IsNullOrEmpty(locacao.Plano) || !Planos.ContainsKey(locacao.Plano))
        {
            return new DevolucaoResult { Status = DevolucaoStatus.InvalidPlan, ErrorMessage = "Plano de locação inválido." };
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

        locacao.FinalizarLocacao(request.DataTermino, custoTotal);
        await _context.SaveChangesAsync();

        return new DevolucaoResult { Status = DevolucaoStatus.Success, CustoTotal = custoTotal };
    }

    private decimal GetValorMultaAdicional(DateTime previsaoTermino, DateTime dataTermino, decimal custoDiario, string plano)
    {
        var diasNaoEfetivados = (previsaoTermino - dataTermino).TotalDays;
        decimal valorDiariasNaoEfetivadas = (decimal)diasNaoEfetivados * custoDiario;
        var multaAdicional = (decimal)0;

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