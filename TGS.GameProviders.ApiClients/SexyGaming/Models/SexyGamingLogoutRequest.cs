using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.SexyGaming.Models.Commons;

namespace TGS.GameProviders.ApiClients.SexyGaming.Models
{
    public record SexyGamingLogoutRequest : SexyGamingBaseRequest
    {
        [JsonPropertyName("userIds")]
        public string PlayerCode { get; set; }
    }
}
