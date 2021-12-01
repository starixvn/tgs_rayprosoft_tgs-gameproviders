using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.PragmaticPlay.Models
{
    public record PragmaticPlayOpenHistoryExtendedResponse : PragmaticPlayErrorResponse
    {
        [JsonPropertyName("url")]
        public string Url { get; init; }
    }
}
