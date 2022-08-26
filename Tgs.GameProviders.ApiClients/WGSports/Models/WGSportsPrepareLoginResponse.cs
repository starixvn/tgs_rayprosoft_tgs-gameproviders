using System.Text.Json.Serialization;
using Tgs.GameProviders.ApiClients.WGSports.Models.Commons;

namespace Tgs.GameProviders.ApiClients.WGSports.Models;

public record WGSportsPrepareLoginResponse : WGSportsResponse
{
    [JsonPropertyName("Token")]
    public string Token { get; init; } = default!;
}
