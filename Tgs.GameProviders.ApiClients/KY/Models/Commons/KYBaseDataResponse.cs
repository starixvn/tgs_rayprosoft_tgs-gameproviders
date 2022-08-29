using System.Text.Json.Serialization;

namespace Tgs.GameProviders.ApiClients.KY.Models.Commons;

public record KYBaseDataResponse
{
    [JsonPropertyName("code")]
    public int Code { get; init; }
}
