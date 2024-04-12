using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.Funky.Client;
using Tgs.GameProviders.Domains;

using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Funky.ApplicationServices;

internal class FunkyRoundDetailGetService(
	IRoundRepository roundService,
	IGameProviderRepository gameProvider,
	IPlayerRepository player,
	IBrandRepository brand,
	IGameRepository game,
	IGameInterfaceFunkyClient client) : IGetService<FunkyRoundDetailQuery, RoundDetailResult>
{
	public async ValueTask<RoundDetailResult> GetAsync(FunkyRoundDetailQuery query, CancellationToken cancellationToken = default)
	{
		var roundInfo = await roundService.GetAsync(
			roundId: query.RoundId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var gameProviderId = await gameProvider.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.Funky,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var playerInfo = await player.GetAsync(
			playerCode: roundInfo.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var gameProviderConfigurationId = await brand.GetBrandGameProviderConfigurationIdAsync(
			gameProviderId: gameProviderId,
			brandId: playerInfo.BrandId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var configuration = await gameProvider.GetFunkyConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await gameProvider.GetGameProviderCultureCodeAsync(
			gameProviderId: gameProviderId,
			cultureCode: query.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "en";
		var gameInfo = await game.GetAsync(
			gameId: roundInfo.GameId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var result = await client.GameWagerDetailAsync(
			apiUrl: configuration.ApiUrl,
			userAgent: configuration.UserAgent,
			authentication: configuration.Authentication,
			playerCode: playerInfo.PlayerCode,
			gameCode: gameInfo.GameExternalId,
			externalWagerId: roundInfo.ExternalRoundId,
			language: cultureCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new RoundDetailResult
		{
			Data = new RoundDetailDataBase[]
			{
				new RoundDetailDataUrl
				{
					Url = result
				}
			}
		};
	}
}
