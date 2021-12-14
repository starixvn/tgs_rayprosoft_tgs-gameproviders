using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.Avia.Models
{
    public class AviaGameRecordInfoResponse
    {
        [JsonPropertyName("list")]
        public AviaGameRecordData[] List { get; set; }

        [JsonPropertyName("PageIndex")]
        public int PageIndex { get; set; }

        [JsonPropertyName("PageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("RecordCount")]
        public int RecordCount { get; set; }
    }
}
