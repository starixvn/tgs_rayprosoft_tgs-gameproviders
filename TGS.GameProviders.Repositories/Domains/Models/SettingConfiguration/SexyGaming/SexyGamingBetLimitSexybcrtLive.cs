using System.Text.Json.Serialization;

namespace TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration.SexyGaming
{
    public record SexyGamingBetLimitSexybcrtLive
    {
        [JsonPropertyName("limitId")]
        public int[] LimitId { get; set; }
    }
}
