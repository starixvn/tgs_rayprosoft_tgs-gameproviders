using System;
using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.Skywind.Models;

public record SkywindUserLoginResponse
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; }
    [JsonPropertyName("key")]
    public string Key { get; set; }
    [JsonPropertyName("username")]
    public string Username { get; set; }
    [JsonPropertyName("lastPasswordUpdate")]
    public DateTime LastPasswordUpdate { get; set; }
}
