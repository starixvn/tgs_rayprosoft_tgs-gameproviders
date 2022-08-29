using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound;

public record EvolutionGamHistoryRoundParticipantHandDecision
{
    [JsonPropertyName("type")]
    public string Type { get; set; }
}
