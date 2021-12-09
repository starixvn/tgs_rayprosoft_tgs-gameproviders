using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.Avia.Models.Commons
{
    internal record class AviaBaseResponse
    {
        [JsonPropertyName("success")]
        public int Success { get; init; }
        [JsonPropertyName("msg")]
        public string Message { get; init; }
    }
}
