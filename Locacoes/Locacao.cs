using Microsoft.EntityFrameworkCore;
using DesafioBackend.Motos;
using DesafioBackend.Entregadores;

namespace DesafioBackend.Locacoes
{
    public class Locacao
    {
        public string? Id { get; private set; }
        public Guid EntregadorId { get; private set; }
        public Guid MotoId { get; private set; }
        public DateTime DataInicio { get; private set; }
        public DateTime DataTermino { get; private set; }
        public DateTime DataPrevisaoTermino { get; private set; }
        public string? Plano { get; private set; }
        public decimal CustoTotal { get; private set; }

        public Locacao() {}

        public Locacao(Guid entregadorid, Guid motoid, DateTime datainicio, DateTime datatermino,
            DateTime dataprevisaotermino, string plano)
        {
            Id = Guid.NewGuid().ToString();
            EntregadorId = entregadorid;
            MotoId = motoid;
            DataInicio = datainicio;
            DataTermino = datatermino;
            DataPrevisaoTermino = dataprevisaotermino;
            Plano = plano;
            CustoTotal = 0;
        }

        public void FinalizarLocacao(DateTime dataTermino, decimal custoTotal)
        {
            DataTermino = dataTermino;
            CustoTotal = custoTotal;
        }
        
        public Moto? Moto { get; set; }
         
        public Entregador? Entregador { get; set; }
    }
}