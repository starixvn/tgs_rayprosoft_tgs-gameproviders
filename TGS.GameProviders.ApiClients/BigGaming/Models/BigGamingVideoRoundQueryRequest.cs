using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.BigGaming.Models.Commons;

namespace TGS.GameProviders.ApiClients.BigGaming.Models
{
    public record BigGamingVideoRoundQueryRequest : BigGamingParamsSignRequest
    {
        [JsonPropertyName("round")]
        public string Round { get; set; }
    }
}
