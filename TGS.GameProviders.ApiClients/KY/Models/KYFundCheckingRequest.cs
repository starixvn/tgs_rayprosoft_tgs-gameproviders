using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.KY.Models.Commons;

namespace TGS.GameProviders.ApiClients.KY.Models
{
    public record KYFundCheckingRequest : KYBaseRequest
    {
        [JsonPropertyName("orderid")]
        public string OrderId { get; init; }
    }
}
