using System.Text.Json.Serialization;
using TGS.GameProviders.Core.LY.Models.Commons;

namespace TGS.GameProviders.Core.LY.Models
{
    public record LYFundCheckingRequest : LYBaseRequest
    {
        [JsonPropertyName("orderid")]
        public string OrderId { get; init; }
    }
}
