using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.BTiService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.BTiSports.ApplicationServices;

public class BTiSportsPlayerFundInGetService : IGetService<BTiSportsPlayerFundInQuery, PlayerFundResult>
{
	private readonly IBrandRepository _brand;
	private readonly IGameInterfaceBTiClient _client;
	private readonly IGameProviderRepository _gameProvider;
	private readonly IPlayerRepository _player;

	public BTiSportsPlayerFundInGetService(
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

	public async ValueTask<PlayerFundResult> GetAsync(BTiSportsPlayerFundInQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.BTiSports).ConfigureAwait(false);
		var player = await _player.GetAsync(query.PlayerCode).ConfigureAwait(false);
		var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
		var configuration = await _gameProvider.GetBTiSportsConfigurationAsync(gameProviderConfigurationId).ConfigureAwait(false);

		var reply = await _client.FundInAsync(
				apiUrl: configuration.ApiUrl,
				agentUserName: configuration.AgentUserName,
				agentPassword: configuration.AgentPassword,
				merchantCustomerCode: player.PlayerCode,
				refTransactionCode: query.OrderId,
				amount: query.Amount,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new PlayerFundResult(
			OrderId: query.OrderId,
			ExternalOrderId: reply.RefTransactionCode);
	}
}
