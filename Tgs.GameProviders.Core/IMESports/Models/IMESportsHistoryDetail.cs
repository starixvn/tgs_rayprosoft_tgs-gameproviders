using System;
using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.IMESports.Models
{
    public record IMESportsHistoryDetail
    {
        [JsonPropertyName("Market")]
        public string Market { get; init; }

        [JsonPropertyName("EventName")]
        public string EventName { get; init; }

        [JsonConverter(typeof(IMESportsDateTimeOffsetConverter))]
        [JsonPropertyName("EventDateTime")]
        public DateTimeOffset EventDateTime { get; init; }

        [JsonPropertyName("CompetitionName")]
        public string CompetitionName { get; init; }

        [JsonPropertyName("HomeTeamName")]
        public string HomeTeamName { get; init; }

        [JsonPropertyName("AwayTeamName")]
        public string AwayTeamName { get; init; }

        [JsonPropertyName("BetType")]
        public string BetType { get; init; }

        [JsonPropertyName("BetTypeDesc")]
        public string BetTypeDesc { get; init; }

        [JsonPropertyName("Period")]
        public string Period { get; init; }

        [JsonPropertyName("Selection")]
        public string BetTypeSelection { get; init; }

        [JsonPropertyName("Odds")]
        public decimal Odds { get; init; }

        [JsonPropertyName("FTScore")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IMESportsFullTimeScore[] FullTimeScores { get; init; }

        [JsonPropertyName("Handicap")]
        public string Handicap { get; init; }

        [JsonPropertyName("Sportsname")]
        public string SportsName { get; init; }

        [JsonPropertyName("EventID")]
        public string EventId { get; init; }

        [JsonPropertyName("Specifier")]
        public string Specifier { get; init; }

        [JsonPropertyName("GameOrder")]
        public string GameOrder { get; init; }

        [JsonPropertyName("IsWagerItemCancelled")]
        public IMESportsIsCancelled IsWagerItemCancelled { get; init; }

        [JsonPropertyName("IsSettled")]
        public IMESportsIsSettled IsSettled { get; init; }

        [JsonPropertyName("IsResettled")]
        public IMESportsIsResettled IsResettled { get; init; }

        [JsonPropertyName("ResultStatus")]
        public IMESportsResultStatus ResultStatus { get; init; }
    }
}
