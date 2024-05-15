using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.Joker.Client;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Joker.ApplicationServices;

internal class JokerRoundDetailGetService(
	IGameBetRepository gameBetRepository,
	IGameProviderRepository gameProviderRepository,
	IGameInterfaceJokerClient client) : IGetService<JokerRoundDetailQuery, RoundDetailResult>
{
	public async ValueTask<RoundDetailResult> GetAsync(
		JokerRoundDetailQuery query,
		CancellationToken cancellationToken = default)
	{
		var gameBetInfo = await gameBetRepository.GetAsync(
			gameBetId: query.GameBetId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var configuration = await gameProviderRepository.GetJokerConfigurationAsync(
			gameProviderConfigurationId: gameBetInfo.GameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await gameProviderRepository.GetGameProviderCultureCodeAsync(
			gameProviderId: gameBetInfo.GameProviderId,
			cultureCode: query.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "en";

		var url = await client.GameRecordDetailsAsync(
			apiUrl: configuration.ApiUrl,
			appId: configuration.AppId,
			secretKey: configuration.SecretKey,
			externalRoundId: gameBetInfo.ExternalBetId,
			language: cultureCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new RoundDetailResult()
		{
			Data = [new RoundDetailDataUrl { Url = url ?? string.Empty }]
		};
	}
}
