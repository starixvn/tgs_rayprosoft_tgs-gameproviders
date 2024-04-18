using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.Joker.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Joker.ApplicationServices;

internal class JokerPlayerFundCheckingGetService : IGetService<JokerPlayerFundCheckingQuery, bool>
{
	private readonly IBrandRepository _brand;
	private readonly IGameInterfaceJokerClient _client;
	private readonly IGameProviderRepository _gameProvider;
	private readonly IPlayerRepository _player;

	public JokerPlayerFundCheckingGetService(
		IBrandRepository brand,
		IGameProviderRepository gameProvider,
		IGameInterfaceJokerClient client,
		IPlayerRepository player)
	{
		_brand = brand ?? throw new ArgumentNullException(nameof(brand));
		_gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
		_client = client ?? throw new ArgumentNullException(nameof(client));
		_player = player ?? throw new ArgumentNullException(nameof(player));
	}

	public async ValueTask<bool> GetAsync(
		JokerPlayerFundCheckingQuery query,
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

		return await _client
			 .FundStatusAsync(
			apiUrl: configuration.ApiUrl,
			 appId: configuration.AppId,
			 secretKey: configuration.SecretKey,
			 transactionId: query.OrderId,
			 cancellationToken: cancellationToken)
			 .ConfigureAwait(false);
	}
}
