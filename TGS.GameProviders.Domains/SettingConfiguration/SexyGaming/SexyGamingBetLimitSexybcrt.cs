using System.Text.Json.Serialization;

namespace TGS.GameProviders.Domains.SettingConfiguration.SexyGaming
{
    public record SexyGamingBetLimitSexybcrt
    {
        [JsonPropertyName("LIVE")]
        public SexyGamingBetLimitSexybcrtLive Live { get; set; }
    }
}
