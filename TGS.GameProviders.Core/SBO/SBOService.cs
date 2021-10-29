using System;
using System.Threading.Tasks;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration;

namespace TGS.GameProviders.Core.SBO
{
    public class SBOService : ISBOService
    {
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly ISettingRepository _settingRepository;

        public SBOService(
            ISettingRepository settingRepository,
            IGameProviderRepository gameProviderRepository)
        {
            _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
            _gameProviderRepository =
                gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        }

        public async ValueTask<SBOConfiguration> GetSettingConfigurationAsync()
            => await _settingRepository.GetConfigurationAsync<SBOConfiguration>(GameProviderConfigurations.SBOBET)
                .ConfigureAwait(false);
    }
}