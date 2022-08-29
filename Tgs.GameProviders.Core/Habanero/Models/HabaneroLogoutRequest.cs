using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.Habanero.Models;

public record HabaneroLogoutRequest
{
    [JsonPropertyName("BrandId")]
    public string BrandId { get; init; }

    [JsonPropertyName("APIKey")]
    public string ApiKey { get; init; }

    [JsonPropertyName("Username")]
    public string PlayerCode { get; init; }

    [JsonPropertyName("password")]
    public string Password => string.Empty;
}
