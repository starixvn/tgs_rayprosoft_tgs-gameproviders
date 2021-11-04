using System.Text.Json.Serialization;
using TGS.GameProviders.Core.LY.Models.Commons;

namespace TGS.GameProviders.Core.LY.Models
{
    public record LYFnudResponse : LYBaseDataResponse
    {
        [JsonPropertyName("money")]
        public string Balance { get; init; }
    }
}
