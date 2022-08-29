using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.Skywind.Models;

public record SkywindGameUrlResponse
{
    [JsonPropertyName("code")]
    public int Code { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; }
    [JsonPropertyName("url")]
    public string Url { get; set; }
    [JsonPropertyName("token")]
    public string Token { get; set; }
}
