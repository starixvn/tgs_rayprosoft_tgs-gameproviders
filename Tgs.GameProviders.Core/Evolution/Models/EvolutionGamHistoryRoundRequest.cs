namespace Tgs.GameProviders.Core.Evolution.Models
{
    public record EvolutionGamHistoryRoundRequest
    {
        public string PlayerCode { get; set; }
        public string RoundId { get; set; }
    }
}
