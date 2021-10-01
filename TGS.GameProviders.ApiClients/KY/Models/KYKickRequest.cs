using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.KY.Models.Commons;

namespace TGS.GameProviders.ApiClients.KY.Models
{
    public record KYKickRequest : KYBaseRequest
    {
        [JsonPropertyName("account")]
        public string PlayerCode { get; init; }
    }
}
