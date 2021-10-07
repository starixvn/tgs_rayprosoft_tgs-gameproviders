using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.GameProviders.GameRecordReceiver.ApplicationServices.TCG
{
    public class TCGGameRecordHandlerResult
    {
        [JsonPropertyName("success")]
        public bool Success { get; init; }
        [JsonPropertyName("lastGameRecorDirectory")]
        public string LastGameRecorDirectory { get; set; }
        [JsonPropertyName("lastGameRecorFile")]
        public string LastGameRecorFile { get; set; }
    }
}
