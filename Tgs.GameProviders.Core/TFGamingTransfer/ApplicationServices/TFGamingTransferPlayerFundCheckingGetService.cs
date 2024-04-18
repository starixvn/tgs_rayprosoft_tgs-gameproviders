using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.TFGamingService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.TFGamingTransfer.ApplicationServices;

public class TFGamingTransferPlayerFundCheckingGetService : IGetService<TFGamingTransferPlayerFundCheckingQuery, bool>
{
	private readonly IBrandRepository _brand;
	private readonly IGameInterfaceTFGamingClient _client;
	private readonly IGameProviderRepository _gameProvider;
	private readonly IPlayerRepository _player;

	public TFGamingTransferPlayerFundCheckingGetService(
		IBrandRepository brand,
		IGameProviderRepository gameProvider,
		IGameInterfaceTFGamingClient client,
		IPlayerRepository player)
	{
		_brand = brand ?? throw new ArgumentNullException(nameof(brand));
		_gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
		_client = client ?? throw new ArgumentNullException(nameof(client));
		_player = player ?? throw new ArgumentNullException(nameof(player));
	}

	public async ValueTask<bool> GetAsync(TFGamingTransferPlayerFundCheckingQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.TFGamingTransfer).ConfigureAwait(false);
		var player = await _player.GetAsync(query.PlayerCode).ConfigureAwait(false);
		var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
		var configuration = await _gameProvider.GetTFGamingTransferConfigurationAsync(gameProviderConfigurationId).ConfigureAwait(false);

		return await _client
			.FundStatusAsync(
			apiUrl: configuration.ApiUrl,
			privateToken: configuration.PrivateToken,
			refTransactionCode: query.OrderId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
