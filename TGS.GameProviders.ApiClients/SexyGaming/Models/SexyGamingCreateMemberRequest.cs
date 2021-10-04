using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.SexyGaming.Models.Commons;

namespace TGS.GameProviders.ApiClients.SexyGaming.Models
{
    public record SexyGamingCreateMemberRequest : SexyGamingBaseRequest
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; }
        [JsonPropertyName("currency")]
        public string Currency { get; set; }
        [JsonPropertyName("betLimit")]
        public string BetLimit { get; set; }
        [JsonPropertyName("language")]
        public string Language { get; set; }
    }
}
