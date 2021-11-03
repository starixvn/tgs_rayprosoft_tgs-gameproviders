using System.Text.Json.Serialization;
using TGS.GameProviders.Core.LY.Models.Commons;

namespace TGS.GameProviders.Core.LY.Models
{
    public record LYGameUrlResponse : LYBaseDataResponse
    {
        [JsonPropertyName("url")]
        public string Url { get; init; }
    }
}
