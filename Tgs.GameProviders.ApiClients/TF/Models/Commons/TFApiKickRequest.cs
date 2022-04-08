using System.Text.Json.Serialization;

namespace Tgs.GameProviders.ApiClients.TF.Models.Commons
{
    public class TFApiKickRequest
    {
        [JsonPropertyName("operator_id")]
        public string PartnerId { get; set; }

        [JsonPropertyName("member")]
        public string PlayerCode { get; set; }
    }
}
