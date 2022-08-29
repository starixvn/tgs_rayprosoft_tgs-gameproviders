using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.Saba.Models;

public record SabaCreateMemberResponse
{
    [JsonPropertyName("error_code")]
    public SabaCreateMemberApiError ErrorCode { get; init; }

    [JsonPropertyName("message")]
    public string ErrorMessage { get; init; }
}
