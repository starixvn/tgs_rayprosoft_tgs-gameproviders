using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.KY.Models.Commons;

namespace TGS.GameProviders.ApiClients.KY.Models
{
    public record KYFnudResponse : KYBaseDataResponse
    {
        [JsonPropertyName("money")]
        public string Balance { get; init; }
    }
}
