using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Client;

namespace Tgs.GameProviders.Core
{
    internal class SettingService : ISettingService
    {
        private readonly IGameProvidersQueryServerClient _gameProvidersQueryServerClient;

        public SettingService(
            IGameProvidersQueryServerClient gameProvidersQueryServerClient
            )
        {
            _gameProvidersQueryServerClient = gameProvidersQueryServerClient ?? throw new ArgumentNullException(nameof(gameProvidersQueryServerClient));
        }

        public ValueTask<TConfiguration> GetConfigurationAsync<TConfiguration>(string key) where TConfiguration : PluggableConfigurationBase
            => _gameProvidersQueryServerClient.SettingGetConfigurationAsync<TConfiguration>(key);
    }
}
