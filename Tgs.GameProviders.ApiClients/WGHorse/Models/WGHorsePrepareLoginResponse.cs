using System.Text.Json.Serialization;
using Tgs.GameProviders.ApiClients.WGHorse.Models.Commons;

namespace Tgs.GameProviders.ApiClients.WGHorse.Models;

public record WGHorsePrepareLoginResponse : WGHorseResponse
{
    [JsonPropertyName("Token")]
    public string Token { get; init; }
}
