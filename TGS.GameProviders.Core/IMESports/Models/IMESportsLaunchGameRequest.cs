// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.IMESports.Models
{
    public record IMESportsLaunchGameRequest
    {
        [JsonPropertyName("MerchantCode")]
        public string MerchantCode { get; init; } = default!;

        [JsonPropertyName("PlayerId")]
        public string IMESPlayerId { get; init; } = default!;

        [JsonPropertyName("GameCode")]
        public string GameId { get; init; } = default!;

        [JsonPropertyName("Language")]
        public string Language { get; init; } = default!;

        [JsonPropertyName("IpAddress")]
        public string IpAddress => "127.0.0.1";

        [JsonPropertyName("ProductWallet")]
        public int ProductWallet { get; init; }

        [JsonPropertyName("Http")]
        public string Http { get; init; } = default!;
    }
}
