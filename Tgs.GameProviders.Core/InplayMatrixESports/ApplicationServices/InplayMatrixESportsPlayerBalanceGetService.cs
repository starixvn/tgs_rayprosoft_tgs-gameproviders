using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.InplayMatrix.Client;
using Tgs.GameInterface.InplayMatrix.Common;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.InplayMatrixESports.ApplicationServices;

public class InplayMatrixESportsPlayerBalanceGetService : IGetService<InplayMatrixESportsPlayerBalanceQuery, PlayerBalanceResult>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceInplayMatrixClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public InplayMatrixESportsPlayerBalanceGetService(
        IGameProviderRepository gameProvider,
        IPlayerRepository player,
        IBrandRepository brand,
        IGameInterfaceInplayMatrixClient client)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _player = player ?? throw new ArgumentNullException(nameof(player));
    }

    public async ValueTask<PlayerBalanceResult> GetAsync(InplayMatrixESportsPlayerBalanceQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider
            .GetGameProviderIdAsync(
                gameProviderCode: GameProviderType.InplayMatrix_ESports,
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
            .GetInplayMatrixESportsConfigurationAsync(
                gameProviderConfigurationId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var reply = await _client
            .PlayerBalanceAsync(
                apiUrl: configuration.ApiUrl,
                merchantCode: configuration.MerchantCode,
                merchantPrefix: configuration.MerchantPrefix,
                playerCode: player.PlayerCode,
                productWallet: (int)InplayMatrixProductWallet.IM_eSports_Wallet,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!reply.IsPlayerExist)
        {
            var currencyCode = await _gameProvider
                .GetGameProviderCurrencyCodeAsync(
                    gameProviderId: gameProviderId,
                    currencyCode: player.DefaultWalletCurrencyCode,
                    cancellationToken)
                .ConfigureAwait(false);

            await _client
                .PlayerCreateAsync(
                    apiUrl: configuration.ApiUrl,
                    merchantCode: configuration.MerchantCode,
                    merchantPrefix: configuration.MerchantPrefix,
                    playerCode: player.PlayerCode,
                    currency: currencyCode,
                    password: configuration.Password,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            reply = await _client
               .PlayerBalanceAsync(
                   apiUrl: configuration.ApiUrl,
                   merchantCode: configuration.MerchantCode,
                   merchantPrefix: configuration.MerchantPrefix,
                   playerCode: player.PlayerCode,
                   productWallet: (int)InplayMatrixProductWallet.IM_eSports_Wallet,
                   cancellationToken: cancellationToken)
               .ConfigureAwait(false);
        }

        return !reply.IsPlayerExist
            ? throw new Exception($"Player does not exist.{player.PlayerCode}")
            : new PlayerBalanceResult
            {
                FreeBalance = reply.Balance.ToDecimal().GetValueOrDefault(0)
            };
    }
}
