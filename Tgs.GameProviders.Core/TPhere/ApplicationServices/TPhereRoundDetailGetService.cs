using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.TPhere.Client;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.TPhere.ApplicationServices;

internal class TPhereRoundDetailGetService(
	IRoundRepository roundRepository,
	IGameProviderRepository gameProviderRepository,
	IGameInterfaceTPhereClient client) : IGetService<TPhereRoundDetailQuery, RoundDetailResult>
{
	public async ValueTask<RoundDetailResult> GetAsync(TPhereRoundDetailQuery query, CancellationToken cancellationToken = default)
	{
		var roundInfo = await roundRepository.GetAsync(
			roundId: query.RoundId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(query.RoundId.ToString(), $"This game bet does not exist.");
		var configuration = await gameProviderRepository.GetTPhereConfiguration(
			gameProviderConfigurationId: roundInfo.GameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await gameProviderRepository.GetGameProviderCultureCodeAsync(
			gameProviderId: roundInfo.GameProviderId,
			cultureCode: query.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "en";

		var result = await client.TP2GameWagerDetailAsync(
			apiUrl: configuration.ApiUrl,
			apiKey: configuration.ApiKey,
			apiToken: configuration.ApiToken,
			externalWagerId: roundInfo.ExternalRoundId,
			externalCreatedOn: roundInfo.ExternalCreatedOn.Value,
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
