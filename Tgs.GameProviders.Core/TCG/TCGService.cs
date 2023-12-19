using System;
using System.Linq;
using System.Threading.Tasks;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.TCG;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.TCG;

namespace Tgs.GameProviders.Core.TCG;

internal class TCGService : ITCGService
{
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly ISettingRepository _settingRepository;

    public TCGService(
        ISettingRepository settingRepository,
        IGameProviderRepository gameProviderRepository)
    {
        _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
        _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
    }

    public async ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode)
    {
        var gameProviderId = await _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.TCG).ConfigureAwait(false);
        var gameProviderCultureCode = await _gameProviderRepository.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);
        return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "EN";
    }

    public async ValueTask<TCGMerchant> GetMerchantAsync(string currencyCode)
    {
        var configuration = await GetSettingConfigurationAsync().ConfigureAwait(false);
        return configuration.Merchants.Where(x => x.CurrencyCode == currencyCode).FirstOrDefault();
    }

    public ValueTask<TCGConfiguration> GetSettingConfigurationAsync()
        => _settingRepository.GetConfigurationAsync<TCGConfiguration>(GameProviderConfigurations.TCG);
}
