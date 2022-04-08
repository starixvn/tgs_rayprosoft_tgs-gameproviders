using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.PragmaticPlay.Models
{
    public record PragmaticPlayOpenHistoryExtendedResponse : PragmaticPlayErrorResponse
    {
        [JsonPropertyName("url")]
        public string Url { get; init; }
    }
}
