using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.GameProviders.GameRecordReceiver.ApplicationServices.TCG
{
    public class TCGGameRecordHandlerQuery
    {
        [JsonPropertyName("subGP")]
        public string SubGP { get; init; }
        [JsonPropertyName("merchantCode")]
        public string MerchantCode { get; init; }
        [JsonPropertyName("isDesignation")]
        public bool IsDesignation { get; init; }
        [JsonPropertyName("lastGameRecorDirectory")]
        public string LastGameRecorDirectory { get; init; }
        [JsonPropertyName("lastGameRecorFile")]
        public string LastGameRecorFile { get; init; }
    }
}
