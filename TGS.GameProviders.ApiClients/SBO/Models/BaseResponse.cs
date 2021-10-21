// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.SBO.Models
{
    public record BaseResponse
    {
        [JsonPropertyName("error")]
        public ApiError Error { get; init; }

        [JsonPropertyName("serverId")]
        public string ServerId { get; init; }

        public record ApiError
        {
            [JsonPropertyName("id")]
            public ServiceApiError Code { get; init; }

            [JsonPropertyName("msg")]
            public string Message { get; init; }
        }
    }
}