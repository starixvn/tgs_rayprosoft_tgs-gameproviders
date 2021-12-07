using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.AllBet.Models;

public record AllBetLoginData
{
    [JsonPropertyName("gameLoginUrl")]
    public string Url { get; init; } = default!;
}