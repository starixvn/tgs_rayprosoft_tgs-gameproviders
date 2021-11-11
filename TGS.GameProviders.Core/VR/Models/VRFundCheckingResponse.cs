using System.Text.Json.Serialization;
using TGS.GameProviders.Core.VR.Models.Commons;

namespace TGS.GameProviders.Core.VR.Models
{
    public record VRFundCheckingResponse
    {
        [JsonPropertyName("totalRecords")]
        public int TotalRecords { get; init; }
        [JsonPropertyName("recordPage")]
        public int RecordPage { get; init; }
        [JsonPropertyName("recordCountPerPage")]
        public int RecordCountPerPage { get; init; }
        [JsonPropertyName("records")]
        public VRFundCheckingRecordResponse[] Records { get; init; }
    }
}
