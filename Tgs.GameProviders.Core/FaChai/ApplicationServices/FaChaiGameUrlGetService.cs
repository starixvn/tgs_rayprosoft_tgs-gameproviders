using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.FaChai.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.FaChai.ApplicationServices;

public class FaChaiGameUrlGetService : IGetService<FaChaiGameUrlQuery, Uri>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceFaChaiClient _client;
    private readonly IGameRepository _game;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public FaChaiGameUrlGetService(
        IGameProviderRepository gameProvider,
        IPlayerRepository player,
        IBrandRepository brand,
        IGameRepository game,
        IGameInterfaceFaChaiClient client)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _game = game ?? throw new ArgumentNullException(nameof(game));
    }

    public async ValueTask<Uri> GetAsync(FaChaiGameUrlQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider
            .GetGameProviderIdAsync(
                gameProviderCode: GameProviderType.FaChai,
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
            .GetFaChaiConfigurationAsync(
                gameProviderConfigurationId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var cultureCode = await _gameProvider
            .GetGameProviderCultureCodeAsync(
                gameProviderId: gameProviderId,
                cultureCode: query.Language,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false) ?? "1";
        var game = await _game
            .GetAsync(
                gameId: query.GameId.Value,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false) ?? throw new Exception($"The game for this game product does not exist.({query.GameId})");

        var response = await _client
            .GameUrlAsync(
                apiUrl: configuration.ApiUrl,
                agentCode: configuration.AgentCode,
                agentKey: configuration.AgentKey,
                currency: configuration.Currency,
                playerCode: player.PlayerCode,
                gameId: int.Parse(game.GameExternalId),
                languageId: int.Parse(string.IsNullOrEmpty(cultureCode) ? "1" : cultureCode),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new Uri(response);
    }
}
