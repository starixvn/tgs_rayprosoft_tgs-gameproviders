// ReSharper disable InconsistentNaming

using System;
using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.WMCasino.Models
{
    public abstract record WMCasinoBaseRequest
    {
        [JsonPropertyName("cmd")]
        public abstract string Action { get; }

        [JsonPropertyName("signature")]
        public string Signature { get; init; } = default!;

        [JsonPropertyName("timestamp")]
        public string Timestamp => DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        [JsonPropertyName("vendorId")]
        public string VendorId { get; init; } = default!;
    }
}
