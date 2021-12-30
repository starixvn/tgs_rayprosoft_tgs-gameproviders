using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.TF.Models.Commons
{
    public class TFApiKickRequest
    {
        [JsonPropertyName("member")]
        public string PlayerCode { get; set; }

        [JsonPropertyName("operator_id")]
        public string PartnerId { get; set; }
    }
}