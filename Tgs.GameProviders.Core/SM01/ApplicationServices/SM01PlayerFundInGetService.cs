using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.SM01Service.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SM01.ApplicationServices;

public class SM01PlayerFundInGetService : IGetService<SM01PlayerFundInQuery, PlayerFundResult>
{
	private readonly IBrandRepository _brand;
	private readonly IGameInterfaceSM01Client _client;
	private readonly IGameProviderRepository _gameProvider;
	private readonly IPlayerRepository _player;

	public SM01PlayerFundInGetService(
		IBrandRepository brand,
		IGameProviderRepository gameProvider,
		IGameInterfaceSM01Client client,
		IPlayerRepository player)
	{
		_brand = brand ?? throw new ArgumentNullException(nameof(brand));
		_gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
		_client = client ?? throw new ArgumentNullException(nameof(client));
		_player = player ?? throw new ArgumentNullException(nameof(player));
	}

	public async ValueTask<PlayerFundResult> GetAsync(SM01PlayerFundInQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.SM01).ConfigureAwait(false);
		var player = await _player.GetAsync(query.PlayerCode).ConfigureAwait(false);
		var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
		var configuration = await _gameProvider.GetSM01ConfigurationAsync(gameProviderConfigurationId).ConfigureAwait(false);

		var reply = await _client
			.FundInAsync(
				apiUrl: configuration.ApiUrl,
				merchantId: configuration.MerchantId,
				merchantKey: configuration.MerchantKey,
				customerName: player.PlayerCode,
				refTransactionCode: query.OrderId,
				amount: int.Parse(query.Amount.ToString()),
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new PlayerFundResult(
			GameProviderTransactionId: query.OrderId,
			ExternalGameProviderTransactionId: reply.RefTransactionCode);
	}
}
