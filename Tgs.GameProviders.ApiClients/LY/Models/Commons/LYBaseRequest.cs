using System.Text.Json.Serialization;

namespace Tgs.GameProviders.ApiClients.LY.Models.Commons;

public record LYBaseRequest
{
    /// <summary>
    /// The action
    /// </summary>
    [JsonPropertyName("s")]
    public int Action { get; set; }
}
