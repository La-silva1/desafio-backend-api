using Microsoft.EntityFrameworkCore;

namespace DesafioBackend.Entregadores
{
    [Index(nameof(Cnpj), IsUnique = true)]
    [Index(nameof(NumeroCNH), IsUnique = true)]
    public class Entregador
    {
        public Guid Id { get; init; }
        public string Nome { get; private set; }
        public string Cnpj { get; private set; }
        public DateTime DataNascimento { get; private set; }
        public string NumeroCNH { get; private set; }
        public string TipoCNH { get; private set; }
        public string? FotoCNH { get; private set; }
        public Entregador()
        {
            Nome = null!;
            Cnpj = null!;
            NumeroCNH = null!;
            TipoCNH = null!;
            FotoCNH = null!;
        }
        public Entregador(string nome, string cnpj, DateTime datanascimento, string numerocnh, string tipocnh, string? fotocnh)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Cnpj = cnpj;
            DataNascimento = datanascimento;
            NumeroCNH = numerocnh;
            TipoCNH = tipocnh;
            FotoCNH = fotocnh;
        }
        public void ModificarFotoCnh(string fotocnh)
        {
            FotoCNH = fotocnh;
        }
    }
}