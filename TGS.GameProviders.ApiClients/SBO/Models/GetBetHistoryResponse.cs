// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.SBO.Models
{
    public record GetBetHistoryResponse : BaseResponse
    {
        [JsonPropertyName("result")]
        public GameResult[] Results { get; init; }
    }
}