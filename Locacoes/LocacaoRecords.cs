using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace DesafioBackend.Locacoes;

public record AddLocacaoRequest
{
    [JsonPropertyName("entregador_id")]
    public required Guid EntregadorId { get; init; }

    [JsonPropertyName("moto_id")]
    public required Guid MotoId { get; init; }

    [JsonPropertyName("data_inicio")]
    public required DateTime DataInicio { get; init; }

    [JsonPropertyName("data_termino")]
    public required DateTime DataTermino { get; init; }

    [JsonPropertyName("data_previsao_termino")]
    public required DateTime DataPrevisaoTermino { get; init; }

    public required string Plano { get; init; }
}
public record DevolverMotoRequest(DateTime DataTermino);
