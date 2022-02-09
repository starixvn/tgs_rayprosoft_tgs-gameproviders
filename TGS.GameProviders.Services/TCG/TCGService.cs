using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.TCG;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.TCG;

namespace TGS.GameProviders.Services.TCG
{
    internal class TCGService : ITCGService
    {
        private readonly IGameProviderService _gameProviderService;
        private readonly IGameService _gameService;
        private readonly ISettingService _settingService;

        public TCGService(
            ISettingService settingService,
            IGameProviderService gameProviderService,
            IGameService gameService
            )
        {
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        }

        public IAsyncEnumerable<GameInfo> GetGameLotteryLobbyAsync(Guid gameProviderId)
            => _gameService.GetGameListAsync(gameProviderId, Core.GameType.LotteryLobby);

        public async ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode)
        {
            var gameProviderId = await _gameProviderService.GetCacheGameProviderIdAsync(GameProviderType.TCG).ConfigureAwait(false);
            var gameProviderCultureCode = await _gameProviderService.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);
            return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "EN";
        }

        public async ValueTask<TCGMerchant> GetMerchantAsync(string currencyCode)
        {
            var configuration = await _settingService.GetConfigurationAsync<TCGConfiguration>(GameProviderConfigurations.TCG).ConfigureAwait(false);
            return configuration.Merchants.Where(x => x.CurrencyCode == currencyCode).FirstOrDefault();
        }

        public async ValueTask<TCGMerchant> GetMerchantByMerchantCodeAsync(string merchantCode)
        {
            var configuration = await _settingService.GetConfigurationAsync<TCGConfiguration>(GameProviderConfigurations.TCG).ConfigureAwait(false);
            return configuration.Merchants.Where(x => x.MerchantCode == merchantCode).FirstOrDefault();
        }

        public ValueTask<TCGConfiguration> GetSettingConfigurationAsync()
            => _settingService.GetConfigurationAsync<TCGConfiguration>(GameProviderConfigurations.TCG);

        public (string SubGP, string MerchantCode) GetSubGPAndMerchantCode(string agentId)
        {
            var splits = agentId.Split("-");
            return (splits[0], splits[1]);
        }

        private string GetAgentId(string subGP, string merchantCode)
                        => string.Format("{0}-{1}", subGP, merchantCode);
    }
}
