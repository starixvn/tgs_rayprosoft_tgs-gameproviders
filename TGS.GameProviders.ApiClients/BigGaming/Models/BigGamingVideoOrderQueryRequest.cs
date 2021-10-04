using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.BigGaming.Models.Commons;

namespace TGS.GameProviders.ApiClients.BigGaming.Models
{
    public record BigGamingVideoOrderQueryRequest : BigGamingParamsSignRequest
    {
        [JsonPropertyName("loginIds")]
        public string[] LoginIds { get; set; }
        [JsonPropertyName("issueId")]
        public string IssueId { get; set; }
    }
}
