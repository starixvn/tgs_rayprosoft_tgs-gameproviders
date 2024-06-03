using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Tgs.GameInterface.Joker.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Joker.ApplicationServices;

internal class JokerGameUrlGetService : IGetService<JokerGameUrlQuery, Uri>
{
	private readonly IBrandRepository _brand;
	private readonly IGameInterfaceJokerClient _client;
	private readonly IGameRepository _game;
	private readonly IGameProviderRepository _gameProvider;
	private readonly IPlayerRepository _player;

	public JokerGameUrlGetService(
		IBrandRepository brand,
		IGameRepository game,
		IGameProviderRepository gameProvider,
		IGameInterfaceJokerClient client,
		IPlayerRepository player)
	{
		_brand = brand ?? throw new ArgumentNullException(nameof(brand));
		_game = game ?? throw new ArgumentNullException(nameof(game));
		_gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
		_client = client ?? throw new ArgumentNullException(nameof(client));
		_player = player ?? throw new ArgumentNullException(nameof(player));
	}

	public async ValueTask<Uri> GetAsync(
		JokerGameUrlQuery query,
		CancellationToken cancellationToken = default)
	{
		var gameProviderId = await _gameProvider
			.GetGameProviderIdAsync(
				gameProviderCode: GameProviderType.Joker,
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
			.GetJokerConfigurationAsync(
				gameProviderConfigurationId: gameProviderConfigurationId,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameToken = await _client.GameTokenAsync(
				apiUrl: configuration.ApiUrl,
				appId: configuration.AppId,
				secretKey: configuration.SecretKey,
				playerCode: query.PlayerCode,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (string.IsNullOrEmpty(gameToken))
		{
			throw new NullReferenceException("Game Token is empty");
		}

		var game = await _game
			.GetAsync(
				gameId: query.GameId.Value,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var cultureCode = await _gameProvider
			.GetGameProviderCultureCodeAsync(
				gameProviderId: gameProviderId,
				cultureCode: query.Language,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "en";

		var queryStrings = HttpUtility.ParseQueryString(string.Empty);
		queryStrings.Add("token", gameToken);
		queryStrings.Add("game", game.GameExternalId);
		queryStrings.Add("mobile", (query.PlatformType == PlatformType.Mobile).ToString().ToLower());
		queryStrings.Add("lang", (!string.IsNullOrEmpty(cultureCode)) ? "en" : cultureCode);

		var url = $"{configuration.GameLaunchUrl.OriginalString}?{queryStrings}";

		return new(url);
	}
}
