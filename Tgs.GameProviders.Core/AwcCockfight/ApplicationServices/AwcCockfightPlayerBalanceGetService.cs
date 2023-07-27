using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcCockfight.ApplicationServices;

public class AwcCockfightPlayerBalanceGetService : IGetService<AwcCockfightPlayerBalanceQuery, PlayerBalanceResult>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceAwcServiceClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public AwcCockfightPlayerBalanceGetService(
        IBrandRepository brand,
        IGameProviderRepository gameProvider,
        IGameInterfaceAwcServiceClient client,
        IPlayerRepository player)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _player = player ?? throw new ArgumentNullException(nameof(player));
    }

    public async ValueTask<PlayerBalanceResult> GetAsync(AwcCockfightPlayerBalanceQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider
            .GetGameProviderIdAsync(
            gameProviderCode: GameProviderType.AWC_COCKFIGHT,
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
            .GetAwcCockfightConfigurationAsync(
            gameProviderConfigurationId: gameProviderConfigurationId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var response = await _client
            .GetBalanceAsync(
            apiUrl: configuration.ApiUrl,
            agentId: configuration.AgentId,
            cert: configuration.Cert,
            playerCode: player.PlayerCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (response.Success && !response.IsPlayerExist)
        {
            var currency = await _gameProvider
                .GetGameProviderCurrencyCodeAsync(
                gameProviderId: gameProviderId,
                currencyCode: player.DefaultWalletCurrencyCode,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            await _client
                .CreatePlayerByBetLimitAsync(
                apiUrl: configuration.ApiUrl,
                agentId: configuration.AgentId,
                cert: configuration.Cert,
                playerCode: query.PlayerCode,
                userName: query.PlayerCode,
                currency: currency,
                betLimit: configuration.BetLimit.ToJsonString(),
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            response = await _client
                .GetBalanceAsync(
                apiUrl: configuration.ApiUrl,
                agentId: configuration.AgentId,
                cert: configuration.Cert,
                playerCode: player.PlayerCode,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        return !response.Success
            ? throw new Exception($"This game product has failed to withdraw the balance..{response.Message}")
            : new PlayerBalanceResult
            {
                FreeBalance = response.Balance.ToDecimal().GetValueOrDefault(0),
                OccupyBalance = 0
            };
    }
}
