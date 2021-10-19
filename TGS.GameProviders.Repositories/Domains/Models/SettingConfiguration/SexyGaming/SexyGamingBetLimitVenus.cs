using System.Text.Json.Serialization;

namespace TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration.SexyGaming
{
    public record SexyGamingBetLimitVenus
    {
        [JsonPropertyName("LIVE")]
        public SexyGamingBetLimitVenusLive Live { get; set; }
    }
}
