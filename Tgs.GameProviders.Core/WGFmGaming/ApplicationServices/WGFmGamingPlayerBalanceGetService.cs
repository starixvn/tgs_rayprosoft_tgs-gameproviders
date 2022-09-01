using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WGFmGaming.ApplicationServices;

public class WGFmGamingPlayerBalanceGetService : IGetService<WGFmGamingPlayerBalanceQuery, decimal?>
{
    private readonly IGameInterfaceWGFmGamingClient _client;
    private readonly ISettingRepository _settingRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly IGameProviderRepository _gameProviderRepository;

    public WGFmGamingPlayerBalanceGetService(
        IGameInterfaceWGFmGamingClient client,
        ISettingRepository settingRepository,
        IPlayerRepository playerRepository,
        IGameProviderRepository gameProviderRepository
        )
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
    }

    public async ValueTask<decimal?> GetAsync(WGFmGamingPlayerBalanceQuery command, CancellationToken cancellationToken = default)
    {
        var playerModel = await _playerRepository.GetAsync(command.PlayerCode).ConfigureAwait(false);
        var configuration = await _settingRepository.GetWGSportsConfigurationAsync().ConfigureAwait(false);

        (bool IsPlayer, decimal balance) = await _client.PlayerBalanceAsync(
            configuration.ApiUrl,
            configuration.PartnerCode,
            configuration.PartnerKey,
            configuration.IV,
            playerModel.PlayerCode,
            cancellationToken).ConfigureAwait(false);

        if (!IsPlayer)
        {
            var gameProviderId = await _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.WGSports).ConfigureAwait(false);

            var currencyCode = await _gameProviderRepository
                .GetGameProviderCurrencyCodeAsync(
                    gameProviderId,
                    playerModel.DefaultWalletCurrencyCode)
                .ConfigureAwait(false)
                ?? throw new Exception($"This game product currency does not exist.({playerModel.DefaultWalletCurrencyCode})");

            _ = await _client
                 .GamePrepareLoginAsync(
                     configuration.ApiUrl,
                     configuration.PartnerCode,
                     configuration.PartnerKey,
                     configuration.IV,
                     playerModel.PlayerCode,
                     currencyCode,
                     cancellationToken)
                 .ConfigureAwait(false);

            (IsPlayer, balance) = await _client.PlayerBalanceAsync(
                configuration.ApiUrl,
                configuration.PartnerCode,
                configuration.PartnerKey,
                configuration.IV,
                playerModel.PlayerCode,
                cancellationToken).ConfigureAwait(false);
        }

        return IsPlayer ? balance : null;
    }
}
