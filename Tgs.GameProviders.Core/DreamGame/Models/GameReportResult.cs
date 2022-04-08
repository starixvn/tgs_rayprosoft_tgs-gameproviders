using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.DreamGame.Models
{
    public record GameReportResult
    {
        /// <summary>
        /// 注单唯一Id
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }
        /// <summary>
        /// 游戏桌号
        /// </summary>
        [JsonPropertyName("tableId")]
        public int TableId { get; set; }
        /// <summary>
        /// 游戏靴号
        /// </summary>
        [JsonPropertyName("shoeId")]
        public long ShoeId { get; set; }
        /// <summary>
        /// 游戏局号
        /// </summary>
        [JsonPropertyName("playId")]
        public long PlayId { get; set; }
        /// <summary>
        /// 游戏大厅号 1，2:旗舰厅； 3，4:现场厅；5.欧美厅,6.彩票厅；7:国际厅
        /// </summary>
        [JsonPropertyName("lobbyId")]
        public int LobbyId { get; set; }
        /// <summary>
        /// 游戏类型
        /// </summary>
        [JsonPropertyName("gameType")]
        public int GameType { get; set; }
        /// <summary>
        /// 游戏Id
        /// </summary>
        [JsonPropertyName("gameId")]
        public int GameId { get; set; }
        /// <summary>
        /// 会员Id
        /// </summary>
        [JsonPropertyName("memberId")]
        public long MemberId { get; set; }
        /// <summary>
        /// 游戏局号
        /// </summary>
        [JsonPropertyName("parentId")]
        public int ParentId { get; set; }
        /// <summary>
        /// 游戏下注时间(北京時間+8)
        /// </summary>
        [JsonPropertyName("betTime")]
        public string BetTime { get; set; }
        /// <summary>
        /// 游戏结算时间(北京時間+8)
        /// </summary>
        [JsonPropertyName("calTime")]
        public string CalTime { get; set; }
        /// <summary>
        /// 派彩金额 (输赢应扣除下注金额)
        /// </summary>
        [JsonPropertyName("winOrLoss")]
        public decimal WinOrLoss { get; set; }
        /// <summary>
        /// 余额
        /// </summary>
        [JsonPropertyName("balanceBefore")]
        public decimal BalanceBefore { get; set; }
        /// <summary>
        /// 下注金额
        /// </summary>
        [JsonPropertyName("betPoints")]
        public decimal BetPoints { get; set; }
        /// <summary>
        /// 好路追注金额
        /// </summary>
        [JsonPropertyName("betPointsz")]
        public decimal BetPointsz { get; set; }
        /// <summary>
        /// 有效下注金额
        /// </summary>
        [JsonPropertyName("availableBet")]
        public decimal AvailableBet { get; set; }
        /// <summary>
        /// 会员登入账号
        /// </summary>
        [JsonPropertyName("userName")]
        public string UserName { get; set; }
        /// <summary>
        /// 游戏结果
        /// </summary>
        [JsonPropertyName("result")]
        public string Result { get; set; }
        /// <summary>
        /// 下注注单
        /// </summary>
        [JsonPropertyName("betDetail")]
        public string BetDetail { get; set; }
        /// <summary>
        /// 下注时客户端IP
        /// </summary>
        [JsonPropertyName("ip")]
        public string Ip { get; set; }
        /// <summary>
        /// 游戏唯一ID
        /// </summary>
        [JsonPropertyName("ext")]
        public string Ext { get; set; }
        /// <summary>
        /// 是否结算：0:未结算, 1:已结算, 2:已撤销(该注单为对冲注单)
        /// </summary>
        [JsonPropertyName("isRevocation")]
        public int IsRevocation { get; set; }
        /// <summary>
        /// 货币ID
        /// </summary>
        [JsonPropertyName("currencyId")]
        public int CurrencyId { get; set; }
        /// <summary>
        /// 下注时客户端类型
        /// </summary>
        [JsonPropertyName("deviceType")]
        public int DeviceType { get; set; }
        /// <summary>
        /// 转账流水号
        /// </summary>
        [JsonPropertyName("serial")]
        public string Serial { get; set; }
        /// <summary>
        ///
        /// </summary>
        [JsonPropertyName("roadid")]
        public int Roadid { get; set; }
        /// <summary>
        /// 追注转账流水号(共享钱包API可用于对账,普通转账API可忽略)
        /// </summary>
        [JsonPropertyName("pluginid")]
        public int Pluginid { get; set; }
    }
}
