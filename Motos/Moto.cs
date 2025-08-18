using Microsoft.EntityFrameworkCore;

namespace DesafioBackend.Motos
{
    [Index(nameof(Placa), IsUnique = true)]
    public class Moto
    {
        public Guid Id { get; init; }
        public int Ano { get; init; }
        public string Modelo { get; private set; }
        public string Placa { get; private set; }

        public Moto(int ano, string modelo, string placa)
        {
            Id = Guid.NewGuid();
            Ano = ano;
            Modelo = modelo;
            Placa = placa;
        }

        public void ModificarPlaca(string placa)
        {
            Placa = placa;
        }
    }
}