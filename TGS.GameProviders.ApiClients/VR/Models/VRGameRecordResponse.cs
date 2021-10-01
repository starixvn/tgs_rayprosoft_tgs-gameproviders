using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.VR.Models.Commons.GameRecord;

namespace TGS.GameProviders.ApiClients.VR.Models
{
    public record VRGameRecordResponse
    {
        [JsonPropertyName("recordCountPerPage")]
        public int Rows { get; init; }
        [JsonPropertyName("recordPage")]
        public int Page { get; init; }
        [JsonPropertyName("totalRecords")]
        public int TotalCount { get; init; }
        [JsonPropertyName("betRecords")]
        public VRGameRecord[] Records { get; init; }
    }
}
