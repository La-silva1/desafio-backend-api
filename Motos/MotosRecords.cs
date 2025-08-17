namespace DesafioBackend.Motos;

public record AddMotoRequest(string Modelo, string Placa, int Ano);
public record UpdateMotoRequest(string Placa);