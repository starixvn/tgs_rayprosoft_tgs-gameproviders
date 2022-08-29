using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.BigGaming.Models;

public record BigGamingVideoOrderQueryItemResponse
{
    [JsonPropertyName("orderId")]
    public long OrderId { get; set; }
    [JsonPropertyName("tranId")]
    public long? TranId { get; set; }
    [JsonPropertyName("sn")]
    public string Sn { get; set; } = default!;
    [JsonPropertyName("uid")]
    public long Uid { get; set; }
    [JsonPropertyName("userId")]
    public long UserId { get; set; }
    [JsonPropertyName("loginId")]
    public string LoginId { get; set; } = default!;
    [JsonPropertyName("moduleId")]
    public int ModuleId { get; set; }
    [JsonPropertyName("moduleName")]
    public string ModuleName { get; set; } = default!;
    [JsonPropertyName("gameId")]
    public int GameId { get; set; }
    [JsonPropertyName("gameName")]
    public string GameName { get; set; } = default!;
    [JsonPropertyName("gameNameEn")]
    public string GameNameEn { get; set; } = default!;
    [JsonPropertyName("orderStatus")]
    public int OrderStatus { get; set; }
    [JsonPropertyName("bAmount")]
    public decimal BAmount { get; set; }
    [JsonPropertyName("aAmount")]
    public decimal? AAmount { get; set; }
    [JsonPropertyName("orderFrom")]
    public int OrderFrom { get; set; }
    [JsonPropertyName("orderTime")]
    public string OrderTime { get; set; } = default!;
    [JsonPropertyName("lastUpdateTime")]
    public string LastUpdateTime { get; set; } = default!;
    [JsonPropertyName("fromIp")]
    public string FromIp { get; set; } = default!;
    [JsonPropertyName("tableId")]
    public string TableId { get; set; } = default!;
    [JsonPropertyName("issueId")]
    public string IssueId { get; set; } = default!;
    [JsonPropertyName("playId")]
    public string PlayId { get; set; } = default!;
    [JsonPropertyName("playName")]
    public string PlayName { get; set; } = default!;
    [JsonPropertyName("playNameEn")]
    public string PlayNameEn { get; set; } = default!;
    [JsonPropertyName("validBet")]
    public decimal ValidBet { get; set; }
    [JsonPropertyName("validAmount")]
    public decimal ValidAmount { get; set; }
    [JsonPropertyName("payment")]
    public decimal Payment { get; set; }
    [JsonPropertyName("noComm")]
    public string NoComm { get; set; } = default!;
}
