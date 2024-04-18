using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.V8Poker.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.V8Poker.ApplicationServices;

public class V8PokerPlayerFundOutGetService : IGetService<V8PokerPlayerFundOutQuery, PlayerFundResult>
{
	private readonly IBrandRepository _brand;
	private readonly IGameInterfaceV8PokerClient _client;
	private readonly IGameProviderRepository _gameProvider;
	private readonly IPlayerRepository _player;
	private readonly ISystemClock _systemClock;

	public V8PokerPlayerFundOutGetService(
		IGameProviderRepository gameProvider,
		IPlayerRepository player,
		IBrandRepository brand,
		ISystemClock systemClock,
		IGameInterfaceV8PokerClient client)
	{
		_gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
		_player = player ?? throw new ArgumentNullException(nameof(player));
		_brand = brand ?? throw new ArgumentNullException(nameof(brand));
		_systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
		_client = client ?? throw new ArgumentNullException(nameof(client));
	}

	public async ValueTask<PlayerFundResult> GetAsync(V8PokerPlayerFundOutQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await _gameProvider.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.V8Poker,
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
		var configuration = await _gameProvider.GetV8PokerConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var orderId = $"{configuration.AgentId}{_systemClock.CurrentTime:yyyyMMddHHmmssfff}{player.PlayerCode}";

		_ = await _client
			.FundOutAsync(
			apiUrl: configuration.ApiUrl,
			deskey: configuration.Deskey,
			md5key: configuration.Md5key,
			agentId: configuration.AgentId,
			playerCode: query.PlayerCode,
			orderId: orderId,
			amount: query.Amount,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new() { OrderId = orderId };
	}
}
