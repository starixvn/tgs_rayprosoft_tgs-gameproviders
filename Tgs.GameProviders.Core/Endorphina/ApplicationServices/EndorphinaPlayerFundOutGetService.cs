using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.EndorphinaService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Endorphina.ApplicationServices;

internal class EndorphinaPlayerFundOutGetService : IGetService<EndorphinaPlayerFundOutQuery, PlayerFundResult>
{
	private readonly IBrandRepository _brand;
	private readonly IGameInterfaceEndorphinaClient _client;
	private readonly IGameProviderRepository _gameProvider;
	private readonly IPlayerRepository _player;

	public EndorphinaPlayerFundOutGetService(
		IBrandRepository brand,
		IGameProviderRepository gameProvider,
		IGameInterfaceEndorphinaClient client,
		IPlayerRepository player)
	{
		_brand = brand ?? throw new ArgumentNullException(nameof(brand));
		_gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
		_client = client ?? throw new ArgumentNullException(nameof(client));
		_player = player ?? throw new ArgumentNullException(nameof(player));
	}

	public async ValueTask<PlayerFundResult> GetAsync(EndorphinaPlayerFundOutQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.Endorphina).ConfigureAwait(false);
		var player = await _player.GetAsync(query.PlayerCode).ConfigureAwait(false);
		var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
		var configuration = await _gameProvider.GetEndorphinaConfigurationAsync(gameProviderConfigurationId).ConfigureAwait(false);
		var gameProviderCurrencyCode = await _gameProvider.GetGameProviderCurrencyCodeAsync(gameProviderId, player.DefaultWalletCurrencyCode).ConfigureAwait(false);

		var reply = await _client
			.FundOutAsync(
				apiUrl: configuration.ApiUrl,
				merchantId: configuration.MerchantId,
				secretKey: configuration.SecretKey,
				username: player.PlayerCode,
				currency: gameProviderCurrencyCode,
				refTransactionCode: query.OrderId,
				amount: query.Amount,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new PlayerFundResult(
			OrderId: query.OrderId,
			ExternalOrderId: reply.TransactionId);
	}
}
