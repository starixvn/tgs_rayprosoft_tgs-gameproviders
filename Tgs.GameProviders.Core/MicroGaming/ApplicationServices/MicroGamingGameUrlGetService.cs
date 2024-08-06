using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.MicroGaming.Client;
using Tgs.GameInterface.MicroGaming.Protos;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.MicroGaming.ApplicationServices;

public class MicroGamingGameUrlGetService(
	IGameProviderRepository gameProvider,
	IPlayerRepository player,
	IBrandRepository brand,
	IGameRepository gameRepository,
	IGameInterfaceMicroGamingClient client) : IGetService<MicroGamingGameUrlQuery, Uri>
{
	public async ValueTask<Uri> GetAsync(MicroGamingGameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProvider.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.MicroGaming,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var playerInfo = await player.GetAsync(
			playerCode: query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var gameProviderConfigurationId = await brand.GetBrandGameProviderConfigurationIdAsync(
			gameProviderId: gameProviderId,
			brandId: playerInfo.BrandId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var configuration = await gameProvider.GetMicroGamingConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await gameProvider.GetGameProviderCultureCodeAsync(
			gameProviderId: gameProviderId,
			cultureCode: query.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "en";
		var gameInfo = await gameRepository.GetAsync(
			gameId: query.GameId.Value,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var reply = await client.GameUrlAsync(
			stsApiBaseUrl: configuration.StsApiUrl,
			clientId: configuration.ClientId,
			clientSecret: configuration.ClientSecret,
			apiBaseUrl: configuration.ApiUrl,
			agentCode: configuration.AgentCode,
			playerCode: playerInfo.PlayerCode,
			language: cultureCode,
			platform: query.PlatformType switch
			{
				PlatformType.Desktop => GamePlatform.Desktop,
				PlatformType.Mobile => GamePlatform.Mobile,
				_ => GamePlatform.Unknown,
			},
			gameCode: gameInfo.GameExternalId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new Uri(reply.GameUrl);
	}
}
