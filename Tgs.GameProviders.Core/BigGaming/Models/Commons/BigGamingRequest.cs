using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.BigGaming.Models.Commons
{
    public record BigGamingRequest
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("method")]
        public string Method { get; set; }
        [JsonPropertyName("params")]
        public JsonDocument Params { get; set; }
        [JsonPropertyName("jsonrpc")]
        public string Jsonrpc { get; set; }
    }
}
