using System.Text.Json.Serialization;
using TGS.GameProviders.Core.BigGaming.Models.Commons;

namespace TGS.GameProviders.Core.BigGaming.Models
{
    public record BigGamingGameRecordFishingRequest : BigGamingParamsSignRequest
    {
        [JsonPropertyName("gameType")]
        public int GameType => 1;
        [JsonPropertyName("startTime")]
        public string StartTime { get; set; } = default!;
        [JsonPropertyName("endTime")]
        public string EndTime { get; set; } = default!;
        [JsonPropertyName("pageIndex")]
        public string PageIndex { get; set; } = default!;
        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }
    }
}
