using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.Funky.Client;
using Tgs.GameProviders.Domains;

using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Funky.ApplicationServices;

internal class FunkyRoundDetailGetService(
	IRoundRepository roundRepository,
	IGameProviderRepository gameProviderRepository,
	IGameRepository gameRepository,
	IGameInterfaceFunkyClient client) : IGetService<FunkyRoundDetailQuery, RoundDetailResult>
{
	public async ValueTask<RoundDetailResult> GetAsync(FunkyRoundDetailQuery query, CancellationToken cancellationToken = default)
	{
		var roundInfo = await roundRepository.GetAsync(
			roundId: query.RoundId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(query.RoundId.ToString(), $"This game bet does not exist.");
		var configuration = await gameProviderRepository.GetFunkyConfigurationAsync(
			gameProviderConfigurationId: roundInfo.GameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await gameProviderRepository.GetGameProviderCultureCodeAsync(
			gameProviderId: roundInfo.GameProviderId,
			cultureCode: query.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "en";
		var gameInfo = await gameRepository.GetAsync(
			gameId: roundInfo.GameId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var result = await client.GameWagerDetailAsync(
			apiUrl: configuration.ApiUrl,
			userAgent: configuration.UserAgent,
			authentication: configuration.Authentication,
			playerCode: roundInfo.PlayerCode,
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
