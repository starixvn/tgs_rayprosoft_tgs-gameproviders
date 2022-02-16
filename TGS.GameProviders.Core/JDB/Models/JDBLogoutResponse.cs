using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.JDB.Models;

public record JDBLogoutResponse
{
    [JsonPropertyName("status")]
    public string Status { get; init; }

    [JsonPropertyName("err_text")]
    public string ErrorMessage { get; init; }
}
