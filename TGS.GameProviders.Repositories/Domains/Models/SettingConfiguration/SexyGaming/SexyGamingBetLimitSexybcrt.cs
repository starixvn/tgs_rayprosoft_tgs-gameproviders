using System.Text.Json.Serialization;

namespace TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration.SexyGaming
{
    public record SexyGamingBetLimitSexybcrt
    {
        [JsonPropertyName("LIVE")]
        public SexyGamingBetLimitSexybcrtLive Live { get; set; }
    }
}
