using System.Text.Json.Serialization;
using Tgs.GameProviders.ApiClients.LY.Models.Commons;

namespace Tgs.GameProviders.ApiClients.LY.Models
{
    public record LYFnudResponse : LYBaseDataResponse
    {
        [JsonPropertyName("money")]
        public string Balance { get; init; }
    }
}
