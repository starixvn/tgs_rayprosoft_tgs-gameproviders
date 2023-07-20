using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.AwcService.Protos;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcCockfight.ApplicationServices;

public class AwcCockfightGameUrlGetService : IGetService<AwcCockfightGameUrlQuery, GameUrlResult>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceAwcServiceClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public AwcCockfightGameUrlGetService(
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

    public async ValueTask<GameUrlResult> GetAsync(AwcCockfightGameUrlQuery query, CancellationToken cancellationToken = default)
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
        var cultureCode = await _gameProvider
            .GetGameProviderCultureCodeAsync(
            gameProviderId: gameProviderId,
            cultureCode: query.Language,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false) ?? "en";

        var request = new GameUrlByLiveLobbyRequest()
        {
            ApiUrl = configuration.ApiUrl.ToString(),
            AgentId = configuration.AgentId,
            Cert = configuration.Cert,
            PlayerCode = query.PlayerCode,
            BackUrl = query.BackUrl.ToString(),
            IsMobileLogin = query.PlatformType == PlatformType.Mobile,
            BetLimit = configuration.BetLimit.ToJsonString(),
            Language = cultureCode
        };

        var response = await _client
            .GameUrlByLiveLobbyByAwcCockfightAsync(
            request: request,
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
                .GameUrlByLiveLobbyByAwcCockfightAsync(
                request: request,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        return (response != null) ? new() { Url = new(response.GameUrl) } : null;
    }
}
