using System.Text.Json.Serialization;
using TGS.GameProviders.Core.KY.Models.Commons;

namespace TGS.GameProviders.Core.KY.Models
{
    public record KYFnudResponse : KYBaseDataResponse
    {
        [JsonPropertyName("money")]
        public string Balance { get; init; }
    }
}
