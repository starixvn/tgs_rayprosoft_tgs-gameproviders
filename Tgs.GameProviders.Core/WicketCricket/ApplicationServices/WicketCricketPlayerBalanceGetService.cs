using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.WicketCricket.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WicketCricket.ApplicationServices;

public class WicketCricketPlayerBalanceGetService : IGetService<WicketCricketPlayerBalanceQuery, PlayerBalanceResult>
{
	private readonly IBrandRepository _brand;
	private readonly IGameInterfaceWicketCricketClient _client;
	private readonly IGameProviderRepository _gameProvider;
	private readonly IPlayerRepository _player;

	public WicketCricketPlayerBalanceGetService(
		IGameProviderRepository gameProvider,
		IPlayerRepository player,
		IBrandRepository brand,
		IGameInterfaceWicketCricketClient client)
	{
		_brand = brand ?? throw new ArgumentNullException(nameof(brand));
		_gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
		_client = client ?? throw new ArgumentNullException(nameof(client));
		_player = player ?? throw new ArgumentNullException(nameof(player));
	}

	public async ValueTask<PlayerBalanceResult> GetAsync(WicketCricketPlayerBalanceQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await _gameProvider
			.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.WicketCricket,
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
			.GetWicketCricketConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var reply = await _client
			.PlayerBalanceAsync(
			apiUrl: configuration.ApiUrl,
			website: configuration.Website,
			cert: configuration.Cert,
			playerCode: player.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!reply.IsPlayerExist)
		{
			var currencyCode = await _gameProvider
				.GetGameProviderCurrencyCodeAsync(
				gameProviderId: gameProviderId,
				currencyCode: player.DefaultWalletCurrencyCode,
				cancellationToken)
				.ConfigureAwait(false);

			await _client
				.PlayerCreateAsync(
				apiUrl: configuration.ApiUrl,
				website: configuration.Website,
				cert: configuration.Cert,
				agent: configuration.Agent,
				playerCode: player.PlayerCode,
				currency: int.Parse(currencyCode),
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			reply = await _client
			   .PlayerBalanceAsync(
			   apiUrl: configuration.ApiUrl,
			   website: configuration.Website,
			   cert: configuration.Cert,
			   playerCode: player.PlayerCode,
			   cancellationToken: cancellationToken)
			   .ConfigureAwait(false);
		}

		return !reply.IsPlayerExist
			? throw new Exception($"Player does not exist.{player.PlayerCode}")
			: new PlayerBalanceResult
			{
				FreeBalance = reply.FreeBalance.ToDecimal().GetValueOrDefault(0),
				OutstandingBalance = reply.OutstandingBalance.ToDecimal().GetValueOrDefault(0)
			};
	}
}
