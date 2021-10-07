using System.Text.Json.Serialization;

namespace TGS.GameProviders.GameRecord.Core.ApplicationServices.TCG
{
    public class TCGGameRecordHandlerResult
    {
        [JsonPropertyName("lastGameRecorDirectory")]
        public string LastGameRecorDirectory { get; set; }

        [JsonPropertyName("lastGameRecorFile")]
        public string LastGameRecorFile { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; init; }
    }
}
