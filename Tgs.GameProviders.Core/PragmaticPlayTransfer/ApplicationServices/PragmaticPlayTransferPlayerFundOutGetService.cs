using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.PragmaticPlay.Client;
using Tgs.GameInterface.WMCasinoService.Protos;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;

internal class PragmaticPlayTransferPlayerFundOutGetService : IGetService<PragmaticPlayTransferPlayerFundOutQuery, PlayerFundResult>
{
	private readonly IBrandRepository _brand;
	private readonly IGameInterfacePragmaticPlayClient _client;
	private readonly IGameProviderRepository _gameProvider;
	private readonly IPlayerRepository _player;

	public PragmaticPlayTransferPlayerFundOutGetService(
		IGameProviderRepository gameProvider,
		IPlayerRepository player,
		IBrandRepository brand,
		IGameInterfacePragmaticPlayClient client)
	{
		_player = player ?? throw new ArgumentNullException(nameof(player));
		_brand = brand ?? throw new ArgumentNullException(nameof(brand));
		_gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
		_client = client ?? throw new ArgumentNullException(nameof(client));
	}

	public async ValueTask<PlayerFundResult> GetAsync(
		PragmaticPlayTransferPlayerFundOutQuery query,
		CancellationToken cancellationToken = default)
	{
		var gameProviderId = await _gameProvider
			.GetGameProviderIdAsync(
				gameProviderCode: GameProviderType.PragmaticPlayTransfer,
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
			.GetPragmaticPlayTransferConfigurationAsync(
				gameProviderConfigurationId: gameProviderConfigurationId,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var reply = await _client.FundOutAsync(
			   apiUrl: configuration.ApiUrl,
			   secureLogin: configuration.SecureLogin,
			   secretKey: configuration.SecretKey,
			   playerCode: query.PlayerCode,
			   orderId: query.OrderId,
			   amount: query.Amount,
			   cancellationToken: cancellationToken)
		   .ConfigureAwait(false);

		return new PlayerFundResult(
			GameProviderTransactionId: query.OrderId,
			ExternalGameProviderTransactionId: reply.TransactionId.ToString());
	}
}
