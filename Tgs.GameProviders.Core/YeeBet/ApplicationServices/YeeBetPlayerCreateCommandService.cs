using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.YeeBet.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.YeeBet.ApplicationServices;

internal class YeeBetPlayerCreateCommandService : ICommandService<YeeBetPlayerCreateCommand>
{
	private readonly IBrandRepository _brand;
	private readonly IGameInterfaceYeeBetClient _client;
	private readonly IGameProviderRepository _gameProvider;
	private readonly IPlayerRepository _player;

	public YeeBetPlayerCreateCommandService(
		IGameProviderRepository gameProvider,
		IPlayerRepository player,
		IBrandRepository brand,
		IGameInterfaceYeeBetClient client)
	{
		_gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
		_player = player ?? throw new ArgumentNullException(nameof(player));
		_brand = brand ?? throw new ArgumentNullException(nameof(brand));
		_client = client ?? throw new ArgumentNullException(nameof(client));
	}

	public async ValueTask ExecuteAsync(YeeBetPlayerCreateCommand command, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await _gameProvider
			.GetGameProviderIdAsync(
				gameProviderCode: GameProviderType.YeeBet,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var player = await _player
			.GetAsync(
				playerCode: command.PlayerCode,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameProviderConfigurationId = await _brand
			.GetBrandGameProviderConfigurationIdAsync(
				gameProviderId: gameProviderId,
				brandId: player.BrandId,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var configuration = await _gameProvider
			.GetYeeBetConfigurationAsync(
				gameProviderConfigurationId: gameProviderConfigurationId,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var currencyCode = player.DefaultWalletCurrencyCode;

		if (!string.IsNullOrWhiteSpace(configuration.SetCurrency))
			currencyCode = configuration.SetCurrency;

		var gameProviderCurrencyCode = await _gameProvider
			.GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode, cancellationToken)
			.ConfigureAwait(false);

		await _client
			.PlayerCreateByBetLimitAsync(
				apiUrl: configuration.ApiUrl,
				appId: configuration.AppId,
				secretKey: configuration.SecretKey,
				clientType: 1,//1 pc ,2 手機 ,3 ipad
				currency: gameProviderCurrencyCode,
				playerCode: player.PlayerCode,
				languageId: 2,//英文
				quotas: configuration.BetLimit,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
