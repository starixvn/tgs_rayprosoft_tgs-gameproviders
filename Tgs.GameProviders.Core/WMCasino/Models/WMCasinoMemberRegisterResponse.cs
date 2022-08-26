// ReSharper disable InconsistentNaming

using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.WMCasino.Models;

public record WMCasinoMemberRegisterResponse : WMCasinoErrorResponse
{
    [JsonPropertyName("result")]
    public string Result { get; init; } = default!;
}
