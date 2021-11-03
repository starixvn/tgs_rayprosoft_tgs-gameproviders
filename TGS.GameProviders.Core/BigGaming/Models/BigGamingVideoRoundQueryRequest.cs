using System.Text.Json.Serialization;
using TGS.GameProviders.Core.BigGaming.Models.Commons;

namespace TGS.GameProviders.Core.BigGaming.Models
{
    public record BigGamingVideoRoundQueryRequest : BigGamingParamsSignRequest
    {
        [JsonPropertyName("round")]
        public string Round { get; set; }
    }
}
