using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.Avia.Models;

public class AviaGameRecordDetail
{
    /// <summary>
    /// 盘口名称
    /// </summary>
    [JsonPropertyName("Bet")]
    public string Bet { get; set; }

    /// <summary>
    /// 盘口ID
    /// </summary>
    [JsonPropertyName("BetID")]
    public string BetID { get; set; }

    /// <summary>
    /// 游戏名称
    /// </summary>
    [JsonPropertyName("Category")]
    public string Category { get; set; }

    /// <summary>
    /// 游戏ID
    /// </summary>
    [JsonPropertyName("CateID")]
    public string CateID { get; set; }

    /// <summary>
    /// 投注内容
    /// </summary>
    [JsonPropertyName("Content")]
    public string Content { get; set; }

    /// <summary>
    /// 子订单ID
    /// </summary>
    [JsonPropertyName("DetailID")]
    public string DetailID { get; set; }

    /// <summary>
    /// 联赛名称
    /// </summary>
    [JsonPropertyName("League")]
    public string League { get; set; }

    /// <summary>
    /// 联赛ID
    /// </summary>
    [JsonPropertyName("LeagueID")]
    public string LeagueID { get; set; }

    /// <summary>
    /// 比赛标题
    /// </summary>
    [JsonPropertyName("Match")]
    public string Match { get; set; }

    /// <summary>
    /// 联赛名称
    /// </summary>
    [JsonPropertyName("MatchID")]
    public string MatchID { get; set; }

    /// <summary>
    /// 赔率（参见附录7：盘口类型）
    /// </summary>
    [JsonPropertyName("Odds")]
    public string Odds { get; set; }

    /// <summary>
    /// 盘口类型（参见附录7：盘口类型）
    /// </summary>
    [JsonPropertyName("OddsType")]
    public string OddsType { get; set; }

    /// <summary>
    /// 开奖结果
    /// </summary>
    [JsonPropertyName("Result")]
    public string Result { get; set; }

    /// <summary>
    /// 结果产生时间
    /// </summary>
    [JsonPropertyName("ResultAt")]
    public string ResultAt { get; set; }

    /// <summary>
    /// 订单状态。详情参见本章节订单状态
    /// </summary>
    [JsonPropertyName("Status")]
    public string Status { get; set; }
}
