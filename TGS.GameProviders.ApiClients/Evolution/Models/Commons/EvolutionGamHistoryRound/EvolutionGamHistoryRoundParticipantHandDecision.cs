using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGamHistoryRound
{
    public record EvolutionGamHistoryRoundParticipantHandDecision
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
