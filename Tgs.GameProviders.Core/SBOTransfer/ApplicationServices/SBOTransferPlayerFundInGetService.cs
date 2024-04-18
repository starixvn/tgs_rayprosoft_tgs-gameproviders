using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.SBOService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SBOTransfer.ApplicationServices;

public class SBOTransferPlayerFundInGetService : IGetService<SBOTransferPlayerFundInQuery, PlayerFundResult>
{
	private readonly IBrandRepository _brandRepository;
	private readonly IGameInterfaceSBOClient _client;
	private readonly IGameProviderRepository _gameProviderRepository;
	private readonly IPlayerRepository _playerRepository;
	private readonly ISnowflakeWork _snowflakeWork;

	public SBOTransferPlayerFundInGetService(
		IGameInterfaceSBOClient client,
		IGameProviderRepository gameProviderRepository,
		IPlayerRepository playerRepository,
		IBrandRepository brandRepository,
		ISnowflakeWork snowflakeWork)
	{
		_client = client ?? throw new ArgumentNullException(nameof(client));
		_gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
		_playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
		_brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
		_snowflakeWork = snowflakeWork ?? throw new ArgumentNullException(nameof(snowflakeWork));
	}

	public async ValueTask<PlayerFundResult> GetAsync(SBOTransferPlayerFundInQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await _gameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.SBOTransfer,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var player = await _playerRepository.GetAsync(
			playerCode: query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var gameProviderConfigurationId = await _brandRepository.GetBrandGameProviderConfigurationIdAsync(
			gameProviderId: gameProviderId,
			player.BrandId,
			cancellationToken)
			.ConfigureAwait(false);
		var configuration = await _gameProviderRepository.GetSBOTransferConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var orderId = $"W{_snowflakeWork.GetId()}";

		var reply = await _client.FundInAsync(
			configuration.ApiUrl,
			configuration.CompanyKey,
			configuration.Agent,
			player.PlayerCode,
			orderId: orderId,
			query.Amount,
			cancellationToken)
			.ConfigureAwait(false);

		return new PlayerFundResult()
		{
			OrderId = reply.TxnId
		};
	}
}
