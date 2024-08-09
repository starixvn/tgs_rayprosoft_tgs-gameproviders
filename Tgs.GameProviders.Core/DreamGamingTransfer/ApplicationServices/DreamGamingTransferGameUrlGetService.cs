using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.DreamGamingService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.DreamGamingTransfer.ApplicationServices;

public class DreamGamingTransferGameUrlGetService(
	IBrandRepository brandRepository,
	IGameProviderRepository gameProviderRepository,
	IGameInterfaceDreamGamingClient client,
	IPlayerRepository playerRepository) : IGetService<DreamGamingTransferGameUrlQuery, string>
{
	public async ValueTask<string> GetAsync(DreamGamingTransferGameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.DreamGamingTransfer,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var player = await playerRepository.GetAsync(
			playerCode: query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var gameProviderConfigurationId = await brandRepository.GetBrandGameProviderConfigurationIdAsync(
			gameProviderId: gameProviderId,
			brandId: player.BrandId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var configuration = await gameProviderRepository.GetDreamGamingTransferConfigurationAsync(
			gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await gameProviderRepository.GetGameProviderCultureCodeAsync(
			gameProviderId: gameProviderId,
			cultureCode: query.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "en";

		var response = await client.GameUrlAsync(
			apiUrl: configuration.ApiUrl,
			apiKey: configuration.ApiKey,
			agentName: configuration.AgentName,
			playerCode: player.PlayerCode,
			password: "DGTFP@ssw0rd",
			isMobile: query.PlatformType == PlatformType.Mobile,
			lang: cultureCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return response.GameUrl;
	}
}
