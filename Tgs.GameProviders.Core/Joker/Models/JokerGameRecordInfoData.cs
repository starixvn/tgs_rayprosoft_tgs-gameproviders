using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.Joker.Models;

public class JokerGameRecordInfoData
{
    [JsonPropertyName("Competition")]
    public IEnumerable<JokerGameRecordData> Competition { get; init; } = Array.Empty<JokerGameRecordData>();

    [JsonPropertyName("Game")]
    public IEnumerable<JokerGameRecordData> Game { get; init; } = Array.Empty<JokerGameRecordData>();

    [JsonPropertyName("Jackpot")]
    public IEnumerable<JokerGameRecordData> Jackpot { get; init; } = Array.Empty<JokerGameRecordData>();
}
