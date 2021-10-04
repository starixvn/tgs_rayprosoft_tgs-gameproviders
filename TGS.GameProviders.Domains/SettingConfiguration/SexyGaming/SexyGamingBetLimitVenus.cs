using System.Text.Json.Serialization;

namespace TGS.GameProviders.Domains.SettingConfiguration.SexyGaming
{
    public record SexyGamingBetLimitVenus
    {
        [JsonPropertyName("LIVE")]
        public SexyGamingBetLimitVenusLive Live { get; set; }
    }
}
