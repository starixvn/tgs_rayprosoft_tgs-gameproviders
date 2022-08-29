using System.Text.Json.Serialization;
using Tgs.GameProviders.Core.SexyGaming.Models.Commons;

namespace Tgs.GameProviders.Core.SexyGaming.Models;

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
