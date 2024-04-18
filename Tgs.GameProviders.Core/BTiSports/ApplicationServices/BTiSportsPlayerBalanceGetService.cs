using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.BTiService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.BTiSports.ApplicationServices;

public class BTiSportsPlayerBalanceGetService : IGetService<BTiSportsPlayerBalanceQuery, PlayerBalanceResult>
{
	private readonly IBrandRepository _brand;
	private readonly IGameInterfaceBTiClient _client;
	private readonly IGameProviderRepository _gameProvider;
	private readonly IPlayerRepository _player;

	public BTiSportsPlayerBalanceGetService(
		IBrandRepository brand,
		IGameProviderRepository gameProvider,
		IGameInterfaceBTiClient client,
		IPlayerRepository player)
	{
		_brand = brand ?? throw new ArgumentNullException(nameof(brand));
		_gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
		_client = client ?? throw new ArgumentNullException(nameof(client));
		_player = player ?? throw new ArgumentNullException(nameof(player));
	}

	public async ValueTask<PlayerBalanceResult> GetAsync(BTiSportsPlayerBalanceQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.BTiSports).ConfigureAwait(false);
		var player = await _player.GetAsync(query.PlayerCode).ConfigureAwait(false);
		var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
		var configuration = await _gameProvider.GetBTiSportsConfigurationAsync(gameProviderConfigurationId).ConfigureAwait(false);

		var reply = await _client
			.PlayerBalanceAsync(
				apiUrl: configuration.ApiUrl,
				agentUserName: configuration.AgentUserName,
				agentPassword: configuration.AgentPassword,
				merchantCustomerCode: player.PlayerCode,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!reply.IsPlayerExist)
		{
			var gameProviderCurrencyCode = await _gameProvider
					.GetGameProviderCurrencyCodeAsync(gameProviderId, player.DefaultWalletCurrencyCode)
					.ConfigureAwait(false)
				?? throw new Exception($"This game product currency does not exist.({player.DefaultWalletCurrencyCode})");

			await _client
				.PlayerCreateAsync(
					apiUrl: configuration.ApiUrl,
					agentUserName: configuration.AgentUserName,
					agentPassword: configuration.AgentPassword,
					brandCode: player.BrandCode,
					merchantCustomerCode: player.PlayerCode,
					currency: gameProviderCurrencyCode,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			reply = await _client
			   .PlayerBalanceAsync(
				   apiUrl: configuration.ApiUrl,
				   agentUserName: configuration.AgentUserName,
				   agentPassword: configuration.AgentPassword,
				   merchantCustomerCode: player.PlayerCode,
				   cancellationToken: cancellationToken)
			   .ConfigureAwait(false);
		}

		return !reply.IsPlayerExist
			? throw new Exception($"Player does not exist.{player.PlayerCode}")
			: new PlayerBalanceResult
			{
				FreeBalance = reply.Balance.ToDecimal().GetValueOrDefault(0)
			};
	}
}
