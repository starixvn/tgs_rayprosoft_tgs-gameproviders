using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.Card365.Model;

public record Card365LogoutRequest
{
    [JsonPropertyName("user")]
    public string PlayerCode { get; init; }

    [JsonPropertyName("cert")]
    public string Cert { get; init; }

    [JsonPropertyName("extension1")]
    public string AgentId { get; init; }
}
