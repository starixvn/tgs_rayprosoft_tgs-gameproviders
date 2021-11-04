using System;

namespace TGS.GameProviders.Domains.SettingConfiguration
{
    public record EvolutionConfiguration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; } = default!;
        public string CasinoID { get; init; } = default!;
        public string Ua2Token { get; init; } = default!;
        public string GameHistoryApiToken { get; init; } = default!;
        public string ExternalLobbyApiToken { get; init; } = default!;
        public Uri Ua2AuthenticationUrl { get; init; } = default!;
        public string OwAuthToken { get; init; } = default!;
    }
}
