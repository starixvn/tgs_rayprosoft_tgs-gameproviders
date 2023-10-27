using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcVenus.ApplicationServices;

public class AwcVenusGameUrlGetService : IGetService<AwcVenusGameUrlQuery, GameUrlResult>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceAwcServiceClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public AwcVenusGameUrlGetService(
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

    public async ValueTask<GameUrlResult> GetAsync(AwcVenusGameUrlQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(
            gameProviderCode: GameProviderType.AWC_VENUS,
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

        var configuration = await _gameProvider.GetAwcVenusConfigurationAsync(
            gameProviderConfigurationId: gameProviderConfigurationId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var cultureCode = await _gameProvider.GetGameProviderCultureCodeAsync(
            gameProviderId: gameProviderId,
            cultureCode: query.Language,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false) ?? "en";

        var reply = await _client
            .GameUrlByAwcVenusAsync(
            apiUrl: configuration.ApiUrl,
            agentId: configuration.AgentId,
            cert: configuration.Cert,
            playerCode: player.PlayerCode,
            isMobileLogin: query.PlatformType == PlatformType.Mobile,
            backUrl: query.BackUrl.ToString(),
            betLimit: configuration.BetLimit.ToJsonString(),
            language: cultureCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!reply.IsPlayerExist)
        {
            var currency = await _gameProvider.GetGameProviderCurrencyCodeAsync(
                gameProviderId: gameProviderId,
                currencyCode: player.DefaultWalletCurrencyCode,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            await _client.PlayerCreateByBetLimitAsync(
                apiUrl: configuration.ApiUrl,
                agentId: configuration.AgentId,
                cert: configuration.Cert,
                playerCode: player.PlayerCode,
                userName: player.PlayerCode,
                currency: currency,
                betLimit: configuration.BetLimit.ToJsonString(),
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            reply = await _client
                .GameUrlByAwcVenusAsync(
                apiUrl: configuration.ApiUrl,
                agentId: configuration.AgentId,
                cert: configuration.Cert,
                playerCode: player.PlayerCode,
                isMobileLogin: query.PlatformType == PlatformType.Mobile,
                backUrl: query.BackUrl.ToString(),
                betLimit: configuration.BetLimit.ToJsonString(),
                language: cultureCode,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        return !reply.IsPlayerExist
            ? throw new Exception($"Player does not exist.({player.PlayerCode})")
            : new GameUrlResult
            {
                Url = new Uri(reply.GameUrl)
            };
    }
}
