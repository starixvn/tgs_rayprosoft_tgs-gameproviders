using System.Text.Json.Serialization;

namespace TGS.GameProviders.Domains.SettingConfiguration.SexyGaming
{
    public record SexyGamingBetLimitVenusLive
    {
        [JsonPropertyName("limitId")]
        public int[] LimitId { get; init; } = default!;
    }
}
