using System.Text.Json.Serialization;
using TGS.GameProviders.Core.KY.Models.Commons;

namespace TGS.GameProviders.Core.KY.Models
{
    public record KYFundCheckingRequest : KYBaseRequest
    {
        [JsonPropertyName("orderid")]
        public string OrderId { get; init; }
    }
}
