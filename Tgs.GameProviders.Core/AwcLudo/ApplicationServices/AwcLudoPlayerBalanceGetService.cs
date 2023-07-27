using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.WMCasinoService.Protos;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcLudo.ApplicationServices;

public class AwcLudoPlayerBalanceGetService : IGetService<AwcLudoPlayerBalanceQuery, PlayerBalanceResult>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceAwcServiceClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public AwcLudoPlayerBalanceGetService(
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

    public async ValueTask<PlayerBalanceResult> GetAsync(AwcLudoPlayerBalanceQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider
            .GetGameProviderIdAsync(
            gameProviderCode: GameProviderType.AWC_LUDO,
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
            .GetAwcLudoConfigurationAsync(
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
            var cultureCode = await _gameProvider
                .GetGameProviderCultureCodeAsync(
                gameProviderId: gameProviderId,
                cultureCode: "en-US",
                cancellationToken: cancellationToken)
                .ConfigureAwait(false) ?? "en";
            var currency = await _gameProvider
                .GetGameProviderCurrencyCodeAsync(
                gameProviderId: gameProviderId,
                currencyCode: player.DefaultWalletCurrencyCode,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            await _client
                .CreatePlayerAsync(
                apiUrl: configuration.ApiUrl,
                agentId: configuration.AgentId,
                cert: configuration.Cert,
                playerCode: query.PlayerCode,
                userName: query.PlayerCode,
                currency: currency,
                language: cultureCode,
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

        return !response.IsPlayerExist
            ? throw new Exception($"This game product has failed to withdraw the balance..{response.Message}")
            : new PlayerBalanceResult
            {
                FreeBalance = response.Balance.ToDecimal().GetValueOrDefault(0),
                OccupyBalance = 0
            };
    }
}
