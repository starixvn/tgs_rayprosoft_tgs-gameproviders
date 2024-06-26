using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.YeeBet.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.YeeBet.ApplicationServices;

internal class YeeBetPlayerCreateCommandService(
	IGameProviderRepository gameProviderRepository,
	IPlayerRepository playerRepository,
	IBrandRepository brandRepository,
	IGameInterfaceYeeBetClient client) : ICommandService<YeeBetPlayerCreateCommand>
{
	public async ValueTask ExecuteAsync(YeeBetPlayerCreateCommand command, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.YeeBet,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var playerInfo = await playerRepository.GetAsync(
			playerCode: command.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var gameProviderConfigurationId = await brandRepository.GetBrandGameProviderConfigurationIdAsync(
			gameProviderId: gameProviderId,
			brandId: playerInfo.BrandId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var configuration = await gameProviderRepository.GetYeeBetConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var currencyCode = playerInfo.DefaultWalletCurrencyCode;

		/*
		 * 配置設定語系轉換拿不同語系幣別
		 * 玩家HKD，SetCurrency配置CNY會使用CNY
		 * 玩家CNY，SetCurrency為空會使用CNY
		 * */
		if (!string.IsNullOrWhiteSpace(configuration.SetCurrency))
			currencyCode = configuration.SetCurrency;

		var gameProviderCurrencyCode = await gameProviderRepository
			.GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode, cancellationToken)
			.ConfigureAwait(false);

		await client.PlayerCreateByBetLimitAsync(
			apiUrl: configuration.ApiUrl,
			appId: configuration.AppId,
			secretKey: configuration.SecretKey,
			clientType: 1,//1 pc ,2 手機 ,3 ipad
			currency: gameProviderCurrencyCode,
			playerCode: playerInfo.PlayerCode,
			languageId: 2,//英文
			quotas: configuration.BetLimit,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
