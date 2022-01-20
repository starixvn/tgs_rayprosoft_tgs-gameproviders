using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.JDB.Models;

public class JDBBetHistoryRequest
{
    [JsonPropertyName("action")]
    public int Action => 54;

    [JsonPropertyName("ts")]
    public long Timestamp { get; init; }

    [JsonPropertyName("parent")]
    public string Parent { get; init; }

    [JsonPropertyName("uid")]
    public string UserId { get; init; }

    [JsonPropertyName("lang")]
    public string Language { get; init; }

    [JsonPropertyName("gType")]
    public int GameType { get; init; }

    [JsonPropertyName("seqNo")]
    public long SequenceNo { get; init; }
}