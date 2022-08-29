using System.Text.Json.Serialization;
using Tgs.GameProviders.Core.BigGaming.Models.Commons;

namespace Tgs.GameProviders.Core.BigGaming.Models;

public record BigGamingVideoRoundQueryRequest : BigGamingParamsSignRequest
{
    [JsonPropertyName("round")]
    public string Round { get; set; }
}
