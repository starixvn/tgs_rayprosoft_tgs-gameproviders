using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.Awc.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcPgs.ApplicationServices;

public class AwcPgsPlayerBalanceGetService : IGetService<AwcPgsPlayerBalanceQuery, PlayerBalanceResult>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceAwcClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public AwcPgsPlayerBalanceGetService(
        IBrandRepository brand,
        IGameProviderRepository gameProvider,
        IGameInterfaceAwcClient client,
        IPlayerRepository player)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _player = player ?? throw new ArgumentNullException(nameof(player));
    }

    public async ValueTask<PlayerBalanceResult> GetAsync(AwcPgsPlayerBalanceQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(
            gameProviderCode: GameProviderType.AWC_PGS,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var player = await _player.GetAsync(
            playerCode: query.PlayerCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(
            gameProviderId: gameProviderId,
            brandId: player.BrandId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var configuration = await _gameProvider.GetAwcPgsConfigurationAsync(
            gameProviderConfigurationId: gameProviderConfigurationId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var reply = await _client.PlayerBalanceAsync(
            apiUrl: configuration.ApiUrl,
            agentId: configuration.AgentId,
            cert: configuration.Cert,
            playerCode: player.PlayerCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!reply.IsPlayerExist)
        {
            var currency = await _gameProvider.GetGameProviderCurrencyCodeAsync(
                gameProviderId: gameProviderId,
                currencyCode: player.DefaultWalletCurrencyCode,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            await _client.PlayerCreateAsync(
                apiUrl: configuration.ApiUrl,
                agentId: configuration.AgentId,
                cert: configuration.Cert,
                playerCode: query.PlayerCode,
                userName: query.PlayerCode,
                currency: currency,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            reply = await _client.PlayerBalanceAsync(
                apiUrl: configuration.ApiUrl,
                agentId: configuration.AgentId,
                cert: configuration.Cert,
                playerCode: player.PlayerCode,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        return !reply.IsPlayerExist
            ? throw new Exception($"Player does not exist.({player.PlayerCode})")
            : new PlayerBalanceResult
            {
                FreeBalance = reply.Balance.ToDecimal().GetValueOrDefault(0)
            };
    }
}
