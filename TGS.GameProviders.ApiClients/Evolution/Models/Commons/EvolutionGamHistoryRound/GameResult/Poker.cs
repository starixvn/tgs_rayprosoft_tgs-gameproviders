using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons.Poker;

namespace TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGamHistoryRound.GameResult
{
    public record Poker
    {
        [JsonPropertyName("bonus")]
        public PokerBonus Bonus { get; set; }
        [JsonPropertyName("cards")]
        public PokerCards Cards { get; set; }
        [JsonPropertyName("dealer")]
        public PokerDealer Dealer { get; set; }
        [JsonPropertyName("outcome")]
        public string Outcome { get; set; }
        [JsonPropertyName("player")]
        public PokerPlayer Player { get; set; }
    }
}
