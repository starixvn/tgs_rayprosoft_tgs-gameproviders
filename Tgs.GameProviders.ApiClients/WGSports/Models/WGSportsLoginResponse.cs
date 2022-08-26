using System.Text.Json.Serialization;
using Tgs.GameProviders.ApiClients.WGSports.Models.Commons;

namespace Tgs.GameProviders.ApiClients.WGSports.Models;

public record WGSportsLoginResponse : WGSportsResponse
{
    [JsonPropertyName("LoginUrl")]
    public string LoginUrl { get; init; } = default!;
    [JsonPropertyName("MemberId")]
    public string MemberId { get; init; } = default!;
}
