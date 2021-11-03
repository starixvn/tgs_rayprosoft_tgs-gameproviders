using System.Text.Json.Serialization;

namespace TGS.GameProviders.Domains.SettingConfiguration.SexyGaming
{
    public record SexyGamingBetLimitSexybcrtLive
    {
        [JsonPropertyName("limitId")]
        public int[] LimitId { get; set; }
    }
}
