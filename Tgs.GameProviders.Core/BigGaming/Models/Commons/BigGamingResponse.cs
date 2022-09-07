using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.BigGaming.Models.Commons;

public record BigGamingResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = default!;
    [JsonPropertyName("result")]
    public JsonDocument Result { get; set; }
    [JsonPropertyName("error")]
    public BigGamingErrorResponse Error { get; set; }
    [JsonPropertyName("jsonrpc")]
    public string Jsonrpc { get; set; } = default!;
}
