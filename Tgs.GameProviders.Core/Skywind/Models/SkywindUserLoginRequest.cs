using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.Skywind.Models;

public record SkywindUserLoginRequest
{
    [JsonPropertyName("secretKey")]
    public string SecretKey { get; set; }
    [JsonPropertyName("username")]
    public string Username { get; set; }
    [JsonPropertyName("password")]
    public string Password { get; set; }
}
