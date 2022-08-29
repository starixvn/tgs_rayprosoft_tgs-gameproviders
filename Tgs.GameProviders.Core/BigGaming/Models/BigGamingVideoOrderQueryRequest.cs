using System.Text.Json.Serialization;
using Tgs.GameProviders.Core.BigGaming.Models.Commons;

namespace Tgs.GameProviders.Core.BigGaming.Models;

public record BigGamingVideoOrderQueryRequest : BigGamingParamsSignRequest
{
    [JsonPropertyName("loginIds")]
    public string[] LoginIds { get; set; }
    [JsonPropertyName("issueId")]
    public string IssueId { get; set; }
}
