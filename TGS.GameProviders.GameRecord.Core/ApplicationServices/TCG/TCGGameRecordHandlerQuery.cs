using System.Text.Json.Serialization;

namespace TGS.GameProviders.GameRecord.Core.ApplicationServices.TCG
{
    public class TCGGameRecordHandlerQuery
    {
        [JsonPropertyName("isDesignation")]
        public bool IsDesignation { get; init; }

        [JsonPropertyName("lastGameRecorDirectory")]
        public string LastGameRecorDirectory { get; init; }

        [JsonPropertyName("lastGameRecorFile")]
        public string LastGameRecorFile { get; init; }

        [JsonPropertyName("merchantCode")]
        public string MerchantCode { get; init; }

        [JsonPropertyName("subGP")]
        public string SubGP { get; init; }
    }
}
