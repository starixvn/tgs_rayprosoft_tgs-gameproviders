#nullable enable

using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.Joker.Models;

public class JokerGameRecordInfoResponse
{
    [JsonPropertyName("nextId")]
    public string? NextId { get; init; }

    [JsonPropertyName("data")]
    public JokerGameRecordInfoData Data { get; init; } = default!;
}