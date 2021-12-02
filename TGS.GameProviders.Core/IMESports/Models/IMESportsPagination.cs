// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.IMESports.Models
{
    public record IMESportsPagination
    {
        [JsonPropertyName("CurrentPage")]
        public int CurrentPage { get; init; }

        [JsonPropertyName("TotalPage")]
        public int TotalPage { get; init; }
    }
}
