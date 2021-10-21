// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.SBO.Models
{
    public record UpdatePlayerBetLimitsRequest : BasePlayerRequest
    {
        [JsonPropertyName("BetSettings")]
        public BetLimit[] BetLimits { get; init; }

        public record BetLimit
        {
            [JsonPropertyName("market_type")]
            public int MarketType { get; init; }

            [JsonPropertyName("max_bet")]
            public int MaxBet { get; init; }

            [JsonPropertyName("max_bet_per_match")]
            public int MaxBetPerMatch { get; init; }

            [JsonPropertyName("min_bet")]
            public int MinBet { get; init; }

            [JsonPropertyName("sport_type")]
            public int SportType { get; init; }
        }
    }
}