using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Tgs.GameInterface.HabaneroService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.HabaneroTransfer.ApplicationServices;

internal class HabaneroTransferGameUrlGetService : IGetService<HabaneroTransferGameUrlQuery, Uri>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceHabaneroClient _client;
    private readonly IGameRepository _game;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public HabaneroTransferGameUrlGetService(
        IBrandRepository brand,
        IGameProviderRepository gameProvider,
        IGameInterfaceHabaneroClient client,
        IPlayerRepository player,
        IGameRepository game)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _game = game ?? throw new ArgumentNullException(nameof(game));
    }

    public async ValueTask<Uri> GetAsync(HabaneroTransferGameUrlQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider
            .GetGameProviderIdAsync(
                gameProviderCode: GameProviderType.HabaneroTransfer,
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
            .GetHabaneroTransferConfigurationAsync(
                gameProviderConfigurationId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var cultureCode = await _gameProvider
            .GetGameProviderCultureCodeAsync(
                gameProviderId,
                query.Language,
                cancellationToken)
            .ConfigureAwait(false) ?? "en";
        var game = await _game
            .GetAsync(
                gameId: query.GameId.Value,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderCurrencyCode = await _gameProvider
            .GetGameProviderCurrencyCodeAsync(
                gameProviderId, player.DefaultWalletCurrencyCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"This game product currency does not exist.({player.DefaultWalletCurrencyCode})");

        var response = await _client
            .GameTokenAsync(
                apiUrl: configuration.ApiUrl,
                brandId: configuration.BrandId,
                apiKey: configuration.ApiKey,
                playerHostAddress: query.IPAddress,
                useragent: query.PlatformType.ToString(),
                username: player.PlayerCode,
                password: "HabaTfP@ssw0rd",
                currencyCode: gameProviderCurrencyCode,
                firstName: player.BrandCode,
                lastName: player.PlayerCode,
                cancellationToken)
            .ConfigureAwait(false);

        var queryStrings = HttpUtility.ParseQueryString(string.Empty);

        queryStrings.Add("brandId", configuration.BrandId);
        queryStrings.Add("keyname", game.GameExternalId);
        queryStrings.Add("token", response.AuthToken);
        queryStrings.Add("mode", "real");
        queryStrings.Add("locale", cultureCode);

        var url = new Uri(configuration.GameApiUrl, $"play?{queryStrings}");

        return url;
    }
}
