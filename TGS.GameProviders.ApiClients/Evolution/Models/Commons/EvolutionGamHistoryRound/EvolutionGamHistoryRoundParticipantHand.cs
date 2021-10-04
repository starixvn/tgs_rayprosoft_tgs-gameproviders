using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGamHistoryRound
{
    public record EvolutionGamHistoryRoundParticipantHand
    {
        [JsonPropertyName("cards")]
        public string[] Cards { get; set; }
        [JsonPropertyName("decisions")]
        public EvolutionGamHistoryRoundParticipantHandDecision[] Decisions { get; set; }
        [JsonPropertyName("outcome")]
        public string Outcome { get; set; }
        [JsonPropertyName("score")]
        public int Score { get; set; }
    }
}
