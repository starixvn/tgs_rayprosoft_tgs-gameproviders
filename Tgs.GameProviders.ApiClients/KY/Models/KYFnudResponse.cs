using System.Text.Json.Serialization;
using Tgs.GameProviders.ApiClients.KY.Models.Commons;

namespace Tgs.GameProviders.ApiClients.KY.Models;

public record KYFnudResponse : KYBaseDataResponse
{
    [JsonPropertyName("money")]
    public string Balance { get; init; } = default!;
}
