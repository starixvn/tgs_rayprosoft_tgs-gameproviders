using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.HabaneroService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.HabaneroTransfer.ApplicationServices;

internal class HabaneroTransferPlayerBalanceGetService : IGetService<HabaneroTransferPlayerBalanceQuery, PlayerBalanceResult>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceHabaneroClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public HabaneroTransferPlayerBalanceGetService(
        IBrandRepository brand,
        IGameProviderRepository gameProvider,
        IGameInterfaceHabaneroClient client,
        IPlayerRepository player)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _player = player ?? throw new ArgumentNullException(nameof(player));
    }

    public async ValueTask<PlayerBalanceResult> GetAsync(HabaneroTransferPlayerBalanceQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.HabaneroTransfer).ConfigureAwait(false);
        var player = await _player.GetAsync(query.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configuration = await _gameProvider.GetHabaneroTransferConfigurationAsync(gameProviderConfigurationId).ConfigureAwait(false);

        var reply = await _client
            .PlayerBalanceAsync(
                apiUrl: configuration.ApiUrl,
                brandId: configuration.BrandId,
                apiKey: configuration.ApiKey,
                username: player.PlayerCode,
                password: "HabaTfP@ssw0rd",
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!reply.IsPlayerExist)
        {
            var gameProviderCurrencyCode = await _gameProvider
                    .GetGameProviderCurrencyCodeAsync(gameProviderId, player.DefaultWalletCurrencyCode)
                    .ConfigureAwait(false)
                ?? throw new Exception($"This game product currency does not exist.({player.DefaultWalletCurrencyCode})");

            var gameTokenReply = await _client
                .GameTokenAsync(
                    apiUrl: configuration.ApiUrl,
                    brandId: configuration.BrandId,
                    apiKey: configuration.ApiKey,
                    playerHostAddress: "127.0.0.1",
                    useragent: PlatformType.Desktop.ToString(),
                    username: player.PlayerCode,
                    password: "HabaTfP@ssw0rd",
                    currencyCode: gameProviderCurrencyCode,
                    firstName: player.BrandCode,
                    lastName: player.PlayerCode,
                    cancellationToken)
                .ConfigureAwait(false);

            if (gameTokenReply.IsPlayerExist)
            {
                reply = await _client
                    .PlayerBalanceAsync(
                    apiUrl: configuration.ApiUrl,
                    brandId: configuration.BrandId,
                    apiKey: configuration.ApiKey,
                    username: player.PlayerCode,
                    password: "HabaTfP@ssw0rd",
                    cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        return !reply.IsPlayerExist
            ? throw new Exception($"Player does not exist.{player.PlayerCode}")
            : new PlayerBalanceResult
            {
                FreeBalance = reply.Balance.ToDecimal().GetValueOrDefault(0)
            };
    }
}
