using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WGFmGaming.ApplicationServices;

public class WGFmGamingPlayerBalanceGetService : IGetService<WGFmGamingPlayerBalanceQuery, decimal?>
{
    private readonly IBrandRepository _brandRepository;
    private readonly IGameInterfaceWGFmGamingClient _client;
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly IPlayerRepository _playerRepository;

    public WGFmGamingPlayerBalanceGetService(
        IGameInterfaceWGFmGamingClient client,
        IGameProviderRepository gameProviderRepository,
        IPlayerRepository playerRepository,
        IBrandRepository brandRepository)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
    }

    public async ValueTask<decimal?> GetAsync(WGFmGamingPlayerBalanceQuery command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.WGSports).ConfigureAwait(false);
        var player = await _playerRepository.GetAsync(command.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brandRepository.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configurationData = await _gameProviderRepository.GetWGFmGamingConfigurationAsync(gameProviderConfigurationId).ConfigureAwait(false);

        (bool IsPlayer, decimal balance) = await _client.PlayerBalanceAsync(
            configurationData.ApiUrl,
            configurationData.PartnerCode,
            configurationData.PartnerKey,
            configurationData.IV,
            player.PlayerCode,
            cancellationToken).ConfigureAwait(false);

        if (!IsPlayer)
        {
            var currencyCode = await _gameProviderRepository
                .GetGameProviderCurrencyCodeAsync(
                    gameProviderId,
                    player.DefaultWalletCurrencyCode)
                .ConfigureAwait(false)
                ?? throw new Exception($"This game product currency does not exist.({player.DefaultWalletCurrencyCode})");

            _ = await _client
                 .GamePrepareLoginAsync(
                     configurationData.ApiUrl,
                     configurationData.PartnerCode,
                     configurationData.PartnerKey,
                     configurationData.IV,
                     player.PlayerCode,
                     currencyCode,
                     cancellationToken)
                 .ConfigureAwait(false);

            (IsPlayer, balance) = await _client.PlayerBalanceAsync(
                configurationData.ApiUrl,
                configurationData.PartnerCode,
                configurationData.PartnerKey,
                configurationData.IV,
                player.PlayerCode,
                cancellationToken).ConfigureAwait(false);
        }

        return IsPlayer ? balance : null;
    }
}
