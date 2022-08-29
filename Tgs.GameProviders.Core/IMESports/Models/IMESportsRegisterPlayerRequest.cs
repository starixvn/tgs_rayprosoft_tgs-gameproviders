// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.IMESports.Models;

public record IMESportsRegisterPlayerRequest
{
    [JsonPropertyName("MerchantCode")]
    public string MerchantCode { get; init; } = default!;

    [JsonPropertyName("PlayerId")]
    public string IMESPlayerId { get; init; } = default!;

    [JsonPropertyName("Currency")]
    public string CurrencyCode { get; init; } = default!;

    [JsonPropertyName("Password")]
    public string Password => "Password";
}
