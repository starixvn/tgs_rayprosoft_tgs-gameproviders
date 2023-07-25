using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.AVIAGaming.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AVIAGaming.ApplicationServices;

public class AVIAGamingGameUrlGetService : IGetService<AVIAGamingGameUrlQuery, string>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceAVIAGamingClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public AVIAGamingGameUrlGetService(
        IGameProviderRepository gameProvider,
        IPlayerRepository player,
        IBrandRepository brand,
        IGameInterfaceAVIAGamingClient client
        )
    {
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<string> GetAsync(AVIAGamingGameUrlQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider
            .GetGameProviderIdAsync(
            gameProviderCode: GameProviderType.AVIAGaming,
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
            .GetAVIAGamingConfigurationAsync(
            gameProviderConfigurationId: gameProviderConfigurationId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var cultureCode = await _gameProvider
            .GetGameProviderCultureCodeAsync(
            gameProviderId: gameProviderId,
            cultureCode: query.Language,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false) ?? "ENG";

        var reply = await _client
             .GameUrlAsync(
             apiUrl: configuration.ApiUrl,
             authorization: configuration.Authorization,
             playerCode: player.PlayerCode,
             lang: cultureCode,
             cancellationToken: cancellationToken)
             .ConfigureAwait(false);

        if (!reply.IsPlayerExist)
        {
            var currencyCode = await _gameProvider
                .GetGameProviderCurrencyCodeAsync(
                gameProviderId: gameProviderId,
                currencyCode: player.DefaultWalletCurrencyCode,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false)
                ?? throw new Exception($"This game product currency does not exist.({player.DefaultWalletCurrencyCode})");

            await _client
                .PlayerCreateAsync(
                apiUrl: configuration.ApiUrl,
                authorization: configuration.Authorization,
                playerCode: player.PlayerCode,
                currency: currencyCode,
                password: configuration.PlayerPassword,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            reply = await _client
                .GameUrlAsync(
                apiUrl: configuration.ApiUrl,
                authorization: configuration.Authorization,
                playerCode: player.PlayerCode,
                lang: cultureCode,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        return reply.GameUrl;
    }
}
