using System;
using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.Avia.Models
{
    public class AviaGameRecordData
    {
        /// <summary>
        /// 主播订单->主播名称
        /// </summary>
        [JsonPropertyName("Anchor")]
        public string Anchor { get; set; }

        /// <summary>
        /// 主播订单->主播ID
        /// </summary>
        [JsonPropertyName("AnchorID")]
        public string AnchorID { get; set; }

        /// <summary>
        /// 盘口名称
        /// </summary>
        [JsonPropertyName("Bet")]
        public string Bet { get; set; }

        /// <summary>
        /// 投注金额
        /// </summary>
        [JsonPropertyName("BetAmount")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal BetAmount { get; set; }

        /// <summary>
        /// 盘口ID
        /// </summary>
        [JsonPropertyName("BetID")]
        public string BetID { get; set; }

        /// <summary>
        /// 有效投注金额
        /// </summary>
        [JsonPropertyName("BetMoney")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal BetMoney { get; set; }

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
        /// 趣味游戏->游戏代码，详情参见本章节（趣味游戏类型）
        /// </summary>
        [JsonPropertyName("Code")]
        public string Code { get; set; }

        /// <summary>
        /// 串关类型
        /// </summary>
        [JsonPropertyName("ComboType")]
        public string ComboType { get; set; }

        /// <summary>
        /// 投注内容
        /// </summary>
        [JsonPropertyName("Content")]
        public string Content { get; set; }

        /// <summary>
        /// 下单时间
        /// </summary>
        [JsonPropertyName("CreateAt")]
        public string CreateAt { get; set; }

        [JsonPropertyName("Currency")]
        public string Currency { get; set; }

        /// <summary>
        /// 串关订单
        /// </summary>
        [JsonPropertyName("Details")]
        public AviaGameRecordDetail[] Details { get; set; }

        /// <summary>
        /// 趣味游戏->期号
        /// </summary>
        [JsonPropertyName("Index")]
        public string Index { get; set; }

        /// <summary>
        /// 投注IP
        /// </summary>
        [JsonPropertyName("IP")]
        public string IP { get; set; }

        /// <summary>
        /// 是否滚球
        /// </summary>
        [JsonPropertyName("IsLive")]
        public int IsLive { get; set; }

        /// <summary>
        /// 是否是测试订单
        /// </summary>
        [JsonPropertyName("IsTest")]
        public int IsTest { get; set; }

        /// <summary>
        /// 语言环境
        /// </summary>
        [JsonPropertyName("Language")]
        public string Language { get; set; }

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
        /// 比赛ID
        /// </summary>
        [JsonPropertyName("MatchID")]
        public string MatchID { get; set; }

        /// <summary>
        /// 盈亏金额
        /// </summary>
        [JsonPropertyName("Money")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal Money { get; set; }

        /// <summary>
        /// 赔率
        /// </summary>
        [JsonPropertyName("Odds")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal Odds { get; set; }

        /// <summary>
        /// 盘口类型（参见附录7：盘口类型）
        /// </summary>
        [JsonPropertyName("OddsType")]
        public string OddsType { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        [JsonPropertyName("OrderID")]
        public string OrderID { get; set; }

        /// <summary>
        /// 设备
        /// </summary>
        [JsonPropertyName("Platform")]
        public string[] Platform { get; set; }

        /// <summary>
        /// 趣味游戏->玩法代码，详情参见本章节（趣味游戏玩法）
        /// </summary>
        [JsonPropertyName("Player")]
        public string Player { get; set; }

        /// <summary>
        /// 本订单重新结算的次数
        /// </summary>
        [JsonPropertyName("ReSettlement")]
        public int ReSettlement { get; set; }

        /// <summary>
        /// 单关订单->开奖结果
        /// </summary>
        [JsonPropertyName("Result")]
        public string Result { get; set; }

        /// <summary>
        /// 结果产生时间
        /// </summary>
        [JsonPropertyName("ResultAt")]
        public string ResultAt { get; set; }

        /// <summary>
        /// 派奖时间
        /// </summary>
        [JsonPropertyName("RewardAt")]
        public string RewardAt { get; set; }

        /// <summary>
        /// 订单状态。详情参见本章节订单状态
        /// </summary>
        [JsonPropertyName("Status")]
        public string Status { get; set; }

        /// <summary>
        /// 订单更新的时间戳
        /// </summary>
        [JsonPropertyName("Timestamp")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long Timestamp { get; set; }

        /// <summary>
        /// 订单类型。详情参见本章节订单类型
        /// </summary>
        [JsonPropertyName("Type")]
        public string Type { get; set; }

        /// <summary>
        /// 订单数据的更新时间
        /// </summary>
        [JsonPropertyName("UpdateAt")]
        public string UpdateAt { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [JsonPropertyName("UserName")]
        public string UserName { get; set; }
    }
}
