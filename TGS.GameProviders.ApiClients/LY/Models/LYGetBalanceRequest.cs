using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.LY.Models.Commons;

namespace TGS.GameProviders.ApiClients.LY.Models
{
    public record LYGetBalanceRequest : LYBaseRequest
    {
        [JsonPropertyName("account")]
        public string PlayerCode { get; init; }
    }
}
