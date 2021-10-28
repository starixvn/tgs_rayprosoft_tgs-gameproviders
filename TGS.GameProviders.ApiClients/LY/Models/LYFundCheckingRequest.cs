using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.LY.Models.Commons;

namespace TGS.GameProviders.ApiClients.LY.Models
{
    public record LYFundCheckingRequest : LYBaseRequest
    {
        [JsonPropertyName("orderid")]
        public string OrderId { get; init; }
    }
}
