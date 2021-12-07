using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.AsiaGaming.Models
{
    public record AsiaGamingCheckOrCreateAccountRequest : AsiaGamingBaseRequest
    {
        [JsonPropertyName("method")]
        public string Method => "lg";
    }
}
