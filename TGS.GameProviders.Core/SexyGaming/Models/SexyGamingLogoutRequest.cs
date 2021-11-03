using System.Text.Json.Serialization;
using TGS.GameProviders.Core.SexyGaming.Models.Commons;

namespace TGS.GameProviders.Core.SexyGaming.Models
{
    public record SexyGamingLogoutRequest : SexyGamingBaseRequest
    {
        [JsonPropertyName("userIds")]
        public string PlayerCode { get; set; }
    }
}
