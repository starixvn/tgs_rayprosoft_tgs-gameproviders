using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound
{
    public record EvolutionGamHistoryRoundParticipantHands
    {
        [JsonPropertyName("hand1")]
        public EvolutionGamHistoryRoundParticipantHand Hand1 { get; set; }
    }
}
