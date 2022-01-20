using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.JDB.Models;

public record JDBGetTokenRequest
{
    [JsonPropertyName("action")]
    public int Action => 21;

    [JsonPropertyName("ts")]
    public long Timestamp { get; init; }

    [JsonPropertyName("parent")]
    public string Parent { get; init; }

    [JsonPropertyName("uid")]
    public string UserId { get; init; }

    [JsonPropertyName("balance")]
    public decimal Balance => 0;

    [JsonPropertyName("lang")]
    public string Language { get; init; }

    [JsonPropertyName("gType")]
    public string GameType { get; init; }

    [JsonPropertyName("mType")]
    public string MachineType { get; init; }

    [JsonPropertyName("windowMode")]
    public int WindowMode => 2;

    [JsonPropertyName("isAPP")]
    public bool IsApp => false;

    [JsonPropertyName("lobbyURL")]
    public string LobbyUrl => string.Empty;

    [JsonPropertyName("moreGame")]
    public int MoreGame { get; init; }

    [JsonPropertyName("jackpotFlag")]
    public int JackpotFlag => 1;

    [JsonPropertyName("isShowDollarSign")]
    public bool IsShowDollarSign => true;
}