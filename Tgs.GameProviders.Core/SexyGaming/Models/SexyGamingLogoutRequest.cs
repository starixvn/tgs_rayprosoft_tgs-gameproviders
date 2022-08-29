using System.Text.Json.Serialization;
using Tgs.GameProviders.Core.SexyGaming.Models.Commons;

namespace Tgs.GameProviders.Core.SexyGaming.Models;

public record SexyGamingLogoutRequest : SexyGamingBaseRequest
{
    [JsonPropertyName("userIds")]
    public string PlayerCode { get; set; }
}
