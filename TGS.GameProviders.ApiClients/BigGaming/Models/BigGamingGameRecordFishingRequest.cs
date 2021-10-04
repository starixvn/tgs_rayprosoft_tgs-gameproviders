using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.BigGaming.Models.Commons;

namespace TGS.GameProviders.ApiClients.BigGaming.Models
{
    public record BigGamingGameRecordFishingRequest : BigGamingParamsSignRequest
    {
        [JsonPropertyName("gameType")]
        public int GameType => 1;
        [JsonPropertyName("startTime")]
        public string StartTime { get; set; }
        [JsonPropertyName("endTime")]
        public string EndTime { get; set; }
        [JsonPropertyName("pageIndex")]
        public string PageIndex { get; set; }
        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }
    }
}
