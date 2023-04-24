using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.PragmaticPlay.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;

internal class PragmaticPlayTransferPlayerBalanceGetService : IGetService<PragmaticPlayTransferPlayerBalanceQuery, decimal>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfacePragmaticPlayClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public PragmaticPlayTransferPlayerBalanceGetService(
        IGameProviderRepository gameProvider,
        IPlayerRepository player,
        IBrandRepository brand,
        IGameInterfacePragmaticPlayClient client)
    {
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<decimal> GetAsync(
        PragmaticPlayTransferPlayerBalanceQuery query,
        CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider
            .GetGameProviderIdAsync(
                gameProviderCode: GameProviderType.PragmaticPlayTransfer,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var player = await _player
            .GetAsync(
                playerCode: query.PlayerCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand
            .GetBrandGameProviderConfigurationIdAsync(
                gameProviderId: gameProviderId,
                brandId: player.BrandId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var configuration = await _gameProvider
            .GetPragmaticPlayTransferConfigurationAsync(
                gameProviderConfigurationId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var response = await _client
            .PlayerBalanceAsync(
                configuration.ApiUrl,
                configuration.SecureLogin,
                configuration.SecretKey,
                query.PlayerCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsPlayerExist)
        {
            var currencyCode = await _gameProvider
                .GetGameProviderCurrencyCodeAsync(
                    gameProviderId: gameProviderId,
                    currencyCode: player.DefaultWalletCurrencyCode,
                    cancellationToken)
                .ConfigureAwait(false);

            await _client.PlayerCreateAsync(
                apiUrl: configuration.ApiUrl,
                secureLogin: configuration.SecureLogin,
                secretKey: configuration.SecretKey,
                playerCode: query.PlayerCode,
                currencyCode: currencyCode,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            response = await _client
               .PlayerBalanceAsync(
                   configuration.ApiUrl,
                   configuration.SecureLogin,
                   configuration.SecretKey,
                   query.PlayerCode,
                   cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        }

        return response.IsPlayerExist ? response.Balance : throw new Exception($"player does not exist.({player.PlayerCode})");
    }
}
