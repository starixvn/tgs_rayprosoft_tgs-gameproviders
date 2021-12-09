using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.Avia.Models
{
    internal record AviaLoginResponse
    {
        [JsonPropertyName("Url")]
        public string Url { get; init; }
    }
}
