// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.SBO.Models;

public record SBOCreatePlayerRequest : SBOBasePlayerRequest
{
    /// <summary>
    /// As per SBO, a agent can only have 1 currency
    /// </summary>
    [JsonPropertyName("Agent")]
    public string AgentId { get; init; }
}
