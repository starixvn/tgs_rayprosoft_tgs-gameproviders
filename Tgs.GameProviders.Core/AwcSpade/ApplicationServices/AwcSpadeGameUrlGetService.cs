using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.Awc.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcSpade.ApplicationServices;

public class AwcSpadeGameUrlGetService : IGetService<AwcSpadeGameUrlQuery, string>
{
	private readonly IBrandRepository _brand;
	private readonly IGameInterfaceAwcClient _client;
	private readonly IGameRepository _game;
	private readonly IGameProviderRepository _gameProvider;
	private readonly IPlayerRepository _player;

	public AwcSpadeGameUrlGetService(
		IBrandRepository brand,
		IGameRepository game,
		IGameProviderRepository gameProvider,
		IGameInterfaceAwcClient client,
		IPlayerRepository player)
	{
		_brand = brand ?? throw new ArgumentNullException(nameof(brand));
		_game = game ?? throw new ArgumentNullException(nameof(game));
		_gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
		_client = client ?? throw new ArgumentNullException(nameof(client));
		_player = player ?? throw new ArgumentNullException(nameof(player));
	}

	public async ValueTask<string> GetAsync(AwcSpadeGameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await _gameProvider.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.AWC_SPADE,
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

		var configuration = await _gameProvider.GetAwcSpadeConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var cultureCode = await _gameProvider.GetGameProviderCultureCodeAsync(
			gameProviderId: gameProviderId,
			cultureCode: query.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "en";

		var game = await _game.GetAsync(
			gameId: query.GameId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var reply = await _client
			.GameUrlByAwcSpadeAsync(
			apiUrl: configuration.ApiUrl,
			agentId: configuration.AgentId,
			cert: configuration.Cert,
			playerCode: player.PlayerCode,
			isMobileLogin: query.PlatformType == PlatformType.Mobile,
			backUrl: query.BackUrl.ToString(),
			externalGameId: game.GameExternalId,
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

			await _client.PlayerCreateAsync(
				apiUrl: configuration.ApiUrl,
				agentId: configuration.AgentId,
				cert: configuration.Cert,
				playerCode: query.PlayerCode,
				userName: query.PlayerCode,
				currency: currency,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			reply = await _client
				.GameUrlByAwcSpadeAsync(
				apiUrl: configuration.ApiUrl,
				agentId: configuration.AgentId,
				cert: configuration.Cert,
				playerCode: player.PlayerCode,
				isMobileLogin: query.PlatformType == PlatformType.Mobile,
				backUrl: query.BackUrl.ToString(),
				externalGameId: game.GameExternalId,
				language: cultureCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}

		return !reply.IsPlayerExist
			? throw new Exception($"Player does not exist.({player.PlayerCode})")
			: reply.GameUrl;
	}
}
