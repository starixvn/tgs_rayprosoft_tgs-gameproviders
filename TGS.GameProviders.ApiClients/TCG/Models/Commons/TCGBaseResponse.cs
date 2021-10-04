using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.TCG.Models.Commons
{
    public record TCGBaseResponse
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }
        [JsonPropertyName("error_desc")]
        public string ErrorDesc { get; set; }
    }
}
