using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.Card365.Model;

public record Card365GetKeyRequest
{
    [JsonPropertyName("cert")]
    public string Cert { get; init; }

    [JsonPropertyName("user")]
    public string PlayerCode { get; init; }

    [JsonPropertyName("userName")]
    public string Username { get; init; }

    [JsonPropertyName("extension1")]
    public string AgentId { get; init; }

    [JsonPropertyName("currency")]
    public string CurrencyCode { get; init; }
}
