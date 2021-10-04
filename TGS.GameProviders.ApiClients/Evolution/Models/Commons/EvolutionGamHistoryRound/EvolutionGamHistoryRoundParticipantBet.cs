using System;
using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGamHistoryRound
{
    public record EvolutionGamHistoryRoundParticipantBet
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("payout")]
        public decimal Payout { get; set; }
        [JsonPropertyName("placedOn")]
        public DateTime PlacedOn { get; set; }
        [JsonPropertyName("stake")]
        public decimal Stake { get; set; }
        [JsonPropertyName("transactionId")]
        public string TransactionId { get; set; }
        [JsonPropertyName("owTransactionId")]
        public string OwTransactionId { get; set; }
    }
}
