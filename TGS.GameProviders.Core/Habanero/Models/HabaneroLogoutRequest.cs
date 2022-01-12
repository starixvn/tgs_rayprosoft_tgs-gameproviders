using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.Habanero.Models;

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

    //[JsonPropertyName("CurrencyCode")]
    //public string CurrencyCode { get; init; }
}