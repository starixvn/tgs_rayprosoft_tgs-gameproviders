using System;
using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.SBO.Models
{
    public record SBOGameResult
    {
        [JsonPropertyName("actualStake")]
        public decimal ActualStake { get; init; }

        [JsonPropertyName("currency")]
        public string Currency { get; init; }

        [JsonPropertyName("ip")]
        public string IpAddress { get; init; }

        [JsonPropertyName("isCustomerTagRisky")]
        public bool IsCustomerTagRisky { get; init; }

        [JsonPropertyName("isHalfWonLose")]
        public bool IsHalfWonLose { get; init; }

        [JsonPropertyName("isLive")]
        public bool IsLive { get; init; }

        [JsonPropertyName("isSystemTagRisky")]
        public bool IsSystemTagRisky { get; init; }

        [JsonPropertyName("maxWinWithoutActualStake")]
        public decimal MaxWinWithoutActualStake { get; init; }

        [JsonPropertyName("modifyDate")]
        public DateTime ModifyDate { get; init; }

        [JsonPropertyName("odds")]
        public decimal Odds { get; init; }

        [JsonPropertyName("oddsStyle")]
        public string OddsStyle { get; init; }

        [JsonPropertyName("orderTime")]
        public DateTime OrderTime { get; init; }

        [JsonPropertyName("refNo")]
        public string ReferenceNo { get; init; }

        [JsonPropertyName("settleTime")]
        public DateTime SettleTime { get; init; }

        [JsonPropertyName("sportsType")]
        public string SportsType { get; init; }

        [JsonPropertyName("stake")]
        public decimal Stake { get; init; }

        [JsonPropertyName("status")]
        public string Status { get; init; }

        [JsonPropertyName("subBet")]
        public SBOSubBet[] SubBets { get; init; }

        [JsonPropertyName("turnover")]
        public decimal Turnover { get; init; }

        [JsonPropertyName("username")]
        public string Username { get; init; }

        [JsonPropertyName("winLost")]
        public decimal WinLose { get; init; }

        [JsonPropertyName("winLostDate")]
        public DateTime WinLostDate { get; init; }

        #region VirtualSports only

        [JsonPropertyName("gameId")]
        public string GameId { get; init; }

        [JsonPropertyName("productType")]
        public string ProductType { get; init; }

        #endregion VirtualSports only
    }
}
