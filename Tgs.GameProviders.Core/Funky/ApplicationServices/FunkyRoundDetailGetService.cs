using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.Funky.Client;

using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Funky.ApplicationServices;

internal class FunkyRoundDetailGetService(
	IGameBetRepository gameBetRepository,
	IGameProviderRepository gameProviderRepository,
	IGameRepository gameRepository,
	IGameInterfaceFunkyClient client) : IGetService<FunkyRoundDetailQuery, RoundDetailResult>
{
	public async ValueTask<RoundDetailResult> GetAsync(FunkyRoundDetailQuery query, CancellationToken cancellationToken = default)
	{
		var gameBetInfo = await gameBetRepository.GetAsync(
			gameBetId: query.GameBetId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var configuration = await gameProviderRepository.GetFunkyConfigurationAsync(
			gameProviderConfigurationId: gameBetInfo.GameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await gameProviderRepository.GetGameProviderCultureCodeAsync(
			gameProviderId: gameBetInfo.GameProviderId,
			cultureCode: query.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "en";
		var gameInfo = await gameRepository.GetAsync(
			gameId: gameBetInfo.GameId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var result = await client.GameWagerDetailAsync(
			apiUrl: configuration.ApiUrl,
			userAgent: configuration.UserAgent,
			authentication: configuration.Authentication,
			playerCode: gameBetInfo.PlayerCode,
			gameCode: gameInfo.GameExternalId,
			externalWagerId: gameBetInfo.ExternalBetId,
			language: cultureCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new RoundDetailResult()
		{
			Data = [new RoundDetailDataUrl { Url = result }]
		};
	}
}
