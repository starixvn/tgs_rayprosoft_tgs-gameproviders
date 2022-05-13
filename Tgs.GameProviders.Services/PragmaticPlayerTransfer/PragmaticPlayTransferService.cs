#nullable enable

using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.PragmaticPlayTransfer;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Services.PragmaticPlayerTransfer;

internal class PragmaticPlayTransferService : IPragmaticPlayTransferService
{
    private readonly IGameProviderService _gameProviderService;
    private readonly IGameService _gameService;
    private readonly ISettingService _settingService;

    public PragmaticPlayTransferService(
        IGameProviderService gameProviderService,
        IGameService gameService,
        ISettingService settingService)
    {
        _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
        _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
    }

    public ValueTask<GameInfo?> GetGameAsync(int gameId)
        => _gameService.GetAsync(gameId);

    public ValueTask<GameProviderInfo> GetGameProviderAsync(Guid gameProviderId)
        => _gameProviderService.GetAsync(gameProviderId);

    public async ValueTask<string?> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode)
    {
        var gameProviderCurrencyCode = await _gameProviderService
            .GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode).ConfigureAwait(false);

        return !string.IsNullOrEmpty(gameProviderCurrencyCode) ? gameProviderCurrencyCode : null;
    }

    public ValueTask<Guid> GetGameProviderIdAsync()
        => _gameProviderService.GetCacheGameProviderIdAsync(GameProviderType.PragmaticPlayTransfer);

    public async ValueTask<string> GetGameProviderLanguageCodeAsync(Guid gameProviderId, string cultureCode)
    {
        var gameProviderCultureCode = await _gameProviderService
            .GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);

        return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "en";
    }

    public ValueTask<PragmaticPlayTransferConfiguration?> GetSettingConfigurationAsync()
        => _settingService.GetConfigurationAsync<PragmaticPlayTransferConfiguration>(
            GameProviderConfigurations.PragmaticPlayTransfer);
}