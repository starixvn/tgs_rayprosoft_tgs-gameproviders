using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.GKXGaming.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.GKXGaming.ApplicationServices;

public class GKXGamingGameUrlGetService : IGetService<GKXGamingGameUrlQuery, string>
{
	private readonly IBrandRepository _brand;
	private readonly IGameInterfaceGKXGamingClient _client;
	private readonly IGameRepository _game;
	private readonly IGameProviderRepository _gameProvider;
	private readonly IPlayerRepository _player;

	public GKXGamingGameUrlGetService(
		IGameProviderRepository gameProvider,
		IPlayerRepository player,
		IBrandRepository brand,
		IGameRepository game,
		IGameInterfaceGKXGamingClient client
		)
	{
		_gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
		_player = player ?? throw new ArgumentNullException(nameof(player));
		_brand = brand ?? throw new ArgumentNullException(nameof(brand));
		_game = game ?? throw new ArgumentNullException(nameof(game));
		_client = client ?? throw new ArgumentNullException(nameof(client));
	}

	public async ValueTask<string> GetAsync(GKXGamingGameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await _gameProvider.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.GKXGaming,
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
		var configuration = await _gameProvider.GetGKXGamingConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var game = await _game.GetAsync(
			gameId: query.GameId.Value,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await _gameProvider.GetGameProviderCultureCodeAsync(
			gameProviderId: gameProviderId,
			cultureCode: query.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "zh-cn";
		return await _client
			.GameUrlAsync(
			apiUrl: configuration.ApiUrl,
			deskey: configuration.Deskey,
			md5key: configuration.Md5key,
			agentId: configuration.AgentId,
			brandCode: player.BrandCode,
			playerCode: player.PlayerCode,
			ipAddress: query.IPAddress,
			externalGameId: game.GameExternalId,
			lang: cultureCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
