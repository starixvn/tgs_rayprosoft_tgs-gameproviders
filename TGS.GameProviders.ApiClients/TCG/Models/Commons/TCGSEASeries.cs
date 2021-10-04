using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.TCG.Models.Commons
{
    public record TCGSEASeries
    {
        [JsonPropertyName("game_group_code")]
        public string GameGroupCode { get; init; }
        [JsonPropertyName("prize_mode_id")]
        public int PrizeModeId { get; init; }
    }
}
