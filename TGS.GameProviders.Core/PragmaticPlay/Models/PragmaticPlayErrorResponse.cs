using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.PragmaticPlay.Models
{
    public record PragmaticPlayErrorResponse
    {
        [JsonPropertyName("error")]
        public string Error { get; init; }

        [JsonPropertyName("description")]
        public string Description { get; init; }
    }
}