using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.BigGaming.Models.Commons;

namespace TGS.GameProviders.ApiClients.BigGaming.Models
{
    public record BigGamingVideoOrderDetailRequest : BigGamingParamsSignRequest
    {
        [JsonPropertyName("orderId")]
        public long OrderId { get; set; }
        [JsonPropertyName("locale")]
        public string Locale { get; set; }
    }
}
