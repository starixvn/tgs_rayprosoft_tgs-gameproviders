using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.YeeBet.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.YeeBet.ApplicationServices;

public class YeeBetGameUrlGetService(
	IGameProviderRepository gameProviderRepository,
	IPlayerRepository playerRepository,
	IBrandRepository brandRepository,
	IGameInterfaceYeeBetClient client,
	ICommandService<YeeBetPlayerCreateCommand> playerCreateService) : IGetService<YeeBetGameUrlQuery, Uri>
{
	public async ValueTask<Uri> GetAsync(YeeBetGameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.YeeBet,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var playerInfo = await playerRepository.GetAsync(
			playerCode: query.PlayerCode,
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
		var cultureCode = await gameProviderRepository.GetGameProviderCultureCodeAsync(
			gameProviderId: gameProviderId,
			cultureCode: query.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "2";

		var isPlayerExist = await client.PlayerUpdateBetLimitAsync(
			apiUrl: configuration.ApiUrl,
			appId: configuration.AppId,
			secretKey: configuration.SecretKey,
			playerCode: playerInfo.PlayerCode,
			quotas: configuration.BetLimit,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!isPlayerExist)
		{
			await playerCreateService
				.ExecuteAsync(
				command: new YeeBetPlayerCreateCommand(
					playerInfo.PlayerCode),
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}

		var response = await client.GameUrlAsync(
			apiUrl: configuration.ApiUrl,
			appId: configuration.AppId,
			secretKey: configuration.SecretKey,
			playerCode: playerInfo.PlayerCode,
			clientType: query.PlatformType == PlatformType.Desktop ? 1 : 2,
			languageId: int.Parse(cultureCode),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new Uri(response);
	}
}
