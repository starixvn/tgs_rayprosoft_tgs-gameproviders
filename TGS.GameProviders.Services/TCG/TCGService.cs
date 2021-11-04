using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Repositories.Models.Game;
using TGS.GameProviders.Core.TCG;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.TCG;

namespace TGS.GameProviders.Services.TCG
{
    internal class TCGService : ITCGService
    {
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGameRepository _gameRepository;
        private readonly ISettingRepository _settingRepository;

        public TCGService(
            ISettingRepository settingRepository,
            IGameProviderRepository gameProviderRepository,
            IGameRepository gameRepository
            )
        {
            _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        }

        public Task<GetGame[]> GetGameLotteryLobbyAsync(Guid gameProviderId)
            => _gameRepository.GetGameList(gameProviderId, GameType.LotteryLobby);

        public string GetGameProviderCode()
            => GameProviderType.TCG;

        public async ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode)
        {
            var gameProviderId = await _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.TCG).ConfigureAwait(false);
            var (IsCultureCode, GameProviderCultureCode) = await _gameProviderRepository.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);
            return (IsCultureCode) ? GameProviderCultureCode : "EN";
        }

        public ValueTask<Guid> GetGameProviderIdAsync()
            => _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.TCG);

        public async ValueTask<TCGMerchant> GetMerchantAsync(string currencyCode)
        {
            var configuration = await _settingRepository.GetConfigurationAsync<TCGConfiguration>(GameProviderConfigurations.TCG).ConfigureAwait(false);
            return configuration.Merchants.Where(x => x.CurrencyCode == currencyCode).FirstOrDefault();
        }

        public async ValueTask<TCGMerchant> GetMerchantByMerchantCodeAsync(string merchantCode)
        {
            var configuration = await _settingRepository.GetConfigurationAsync<TCGConfiguration>(GameProviderConfigurations.TCG).ConfigureAwait(false);
            return configuration.Merchants.Where(x => x.MerchantCode == merchantCode).FirstOrDefault();
        }

        public Task<TCGConfiguration> GetSettingConfigurationAsync()
            => _settingRepository.GetConfigurationAsync<TCGConfiguration>(GameProviderConfigurations.TCG);

        public (string SubGP, string MerchantCode) GetSubGPAndMerchantCode(string agentId)
        {
            var splits = agentId.Split("-");
            return (splits[0], splits[1]);
        }

        private string GetAgentId(string subGP, string merchantCode)
                        => string.Format("{0}-{1}", subGP, merchantCode);
    }
}
