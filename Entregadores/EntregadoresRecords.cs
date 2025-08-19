using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http; 
using DesafioBackend.Entregadores;

namespace DesafioBackend.Entregadores;

public record AddEntregadorRequest
{
    public required string Nome { get; init; }
    public required string Cnpj { get; init; }

    [JsonPropertyName("data_nascimento")]
    public required DateTime DataNascimento { get; init; }

    [JsonPropertyName("numero_cnh")]
    public required string NumeroCNH { get; init; }

    [JsonPropertyName("tipo_cnh")]
    public required string TipoCNH { get; init; }

    [JsonPropertyName("foto_cnh")]
    public string FotoCNH { get; init; } = string.Empty;
}
public record AddEntregadorCnh
{
    [JsonPropertyName("imagem_cnh")]
    public required string ImagemCnh { get; init; }
}
