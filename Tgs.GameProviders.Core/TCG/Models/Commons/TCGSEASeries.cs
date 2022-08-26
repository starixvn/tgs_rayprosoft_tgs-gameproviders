using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.TCG.Models.Commons;

public record TCGSEASeries
{
    [JsonPropertyName("game_group_code")]
    public string GameGroupCode { get; init; }
    [JsonPropertyName("prize_mode_id")]
    public int PrizeModeId { get; init; }
}
