using Tgs.GameProviders.Core;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Client;

namespace Tgs.GameProviders.Repositories
{
    internal class SettingRepository : ISettingRepository
    {
        private readonly IGameProvidersQueryServerClient _client;

        public SettingRepository(
            IGameProvidersQueryServerClient client
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public ValueTask<AwcAelottoConfiguration> GetAwcAelottoConfigurationAsync()
            => GetAsync<AwcAelottoConfiguration>(GameProviderConfigurations.AWC_AELOTTO);

        public ValueTask<AwcAwsConfiguration> GetAwcAwsConfigurationAsync()
            => GetAsync<AwcAwsConfiguration>(GameProviderConfigurations.AWC_AWS);

        public ValueTask<AwcCockfightConfiguration> GetAwcCockfightConfigurationAsync()
            => GetAsync<AwcCockfightConfiguration>(GameProviderConfigurations.AWC_COCKFIGHT);

        public ValueTask<AwcFacaiConfiguration> GetAwcFacaiConfigurationAsync()
            => GetAsync<AwcFacaiConfiguration>(GameProviderConfigurations.AWC_FACAI);

        public ValueTask<AwcFsConfiguration> GetAwcFsConfigurationAsync()
            => GetAsync<AwcFsConfiguration>(GameProviderConfigurations.AWC_FS);

        public ValueTask<AwcKmConfiguration> GetAwcKmConfigurationAsync()
            => GetAsync<AwcKmConfiguration>(GameProviderConfigurations.AWC_KM);

        public ValueTask<AwcLudoConfiguration> GetAwcLudoConfigurationAsync()
            => GetAsync<AwcLudoConfiguration>(GameProviderConfigurations.AWC_LUDO);

        public ValueTask<AwcPgsConfiguration> GetAwcPgsConfigurationAsync()
            => GetAsync<AwcPgsConfiguration>(GameProviderConfigurations.AWC_PGS);

        public ValueTask<AwcRcbConfiguration> GetAwcRcbConfigurationAsync()
            => GetAsync<AwcRcbConfiguration>(GameProviderConfigurations.AWC_RCB);

        public ValueTask<AwcSgConfiguration> GetAwcSgConfigurationAsync()
           => GetAsync<AwcSgConfiguration>(GameProviderConfigurations.AWC_SG);

        public ValueTask<AwcSpadeConfiguration> GetAwcSpadeConfigurationAsync()
            => GetAsync<AwcSpadeConfiguration>(GameProviderConfigurations.AWC_SPADE);

        public ValueTask<AwcVenusConfiguration> GetAwcVenusConfigurationAsync()
            => GetAsync<AwcVenusConfiguration>(GameProviderConfigurations.AWC_VENUS);

        public ValueTask<AwcYlConfiguration> GetAwcYlConfigurationAsync()
            => GetAsync<AwcYlConfiguration>(GameProviderConfigurations.AWC_YL);

        public ValueTask<JiliTransferConfiguration> GetJiliTransferConfigurationAsync()
            => GetAsync<JiliTransferConfiguration>(GameProviderConfigurations.JiliTransfer);

        public ValueTask<WMCasinoTransferConfiguration> GetWMCasinoTransferConfigurationAsync()
            => GetAsync<WMCasinoTransferConfiguration>(GameProviderConfigurations.WMCasinoTransfer);

        public ValueTask<EvolutionTransferConfiguration> GetEvolutionTransferConfigurationAsync()
            => GetAsync<EvolutionTransferConfiguration>(GameProviderConfigurations.EvolutionTransfer);

        public ValueTask<TConfiguration> GetConfigurationAsync<TConfiguration>(string key) where TConfiguration : PluggableConfigurationBase
            => GetAsync<TConfiguration>(key);

        private async ValueTask<TConfiguration> GetAsync<TConfiguration>(string key)
            => await _client.SettingGetConfigurationAsync<TConfiguration>(key).ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({key})");
        public ValueTask<IDNLiveConfiguration> GetIDNLiveConfigurationAsync()
            => GetAsync<IDNLiveConfiguration>(GameProviderConfigurations.IDNLive);
    }
}