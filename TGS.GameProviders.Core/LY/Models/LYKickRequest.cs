using System.Text.Json.Serialization;
using TGS.GameProviders.Core.LY.Models.Commons;

namespace TGS.GameProviders.Core.LY.Models
{
    public record LYKickRequest : LYBaseRequest
    {
        [JsonPropertyName("account")]
        public string PlayerCode { get; init; }
    }
}
