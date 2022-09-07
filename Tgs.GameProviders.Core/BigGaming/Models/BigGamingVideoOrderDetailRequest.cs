using System.Text.Json.Serialization;
using Tgs.GameProviders.Core.BigGaming.Models.Commons;

namespace Tgs.GameProviders.Core.BigGaming.Models;

public record BigGamingVideoOrderDetailRequest : BigGamingParamsSignRequest
{
    [JsonPropertyName("orderId")]
    public long OrderId { get; init; }
    [JsonPropertyName("locale")]
    public string Locale { get; init; } = default!;
}
