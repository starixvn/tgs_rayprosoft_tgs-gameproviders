using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.TPhere.Client;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.TPhere.ApplicationServices;

internal class TPhereRoundDetailGetService(
	IGameBetRepository gameBetRepository,
	IGameProviderRepository gameProviderRepository,
	IGameInterfaceTPhereClient client) : IGetService<TPhereRoundDetailQuery, RoundDetailResult>
{
	public async ValueTask<RoundDetailResult> GetAsync(TPhereRoundDetailQuery query, CancellationToken cancellationToken = default)
	{
		var gameBetInfo = await gameBetRepository.GetAsync(
			gameBetId: query.GameBetId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var configuration = await gameProviderRepository.GetTPhereConfigurationAsync(
			gameProviderConfigurationId: gameBetInfo.GameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await gameProviderRepository.GetGameProviderCultureCodeAsync(
			gameProviderId: gameBetInfo.GameProviderId,
			cultureCode: query.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "en";

		var result = await client.TP2GameWagerDetailAsync(
			apiUrl: configuration.ApiUrl,
			apiKey: configuration.ApiKey,
			apiToken: configuration.ApiToken,
			externalWagerId: gameBetInfo.ExternalBetId,
			externalCreatedOn: gameBetInfo.ExternalCreatedOn,
			language: cultureCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new RoundDetailResult()
		{
			Data = [new RoundDetailDataUrl { Url = result ?? string.Empty }]
		};
	}
}
