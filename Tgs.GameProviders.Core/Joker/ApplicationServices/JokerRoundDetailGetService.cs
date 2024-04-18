using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.Joker.Client;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Joker.ApplicationServices;

internal class JokerRoundDetailGetService(
	IGameProviderRepository gameProviderRepository,
	IGameInterfaceJokerClient client,
	IRoundRepository roundRepository) : IGetService<JokerRoundDetailQuery, RoundDetailResult>
{
	public async ValueTask<RoundDetailResult> GetAsync(
		JokerRoundDetailQuery query,
		CancellationToken cancellationToken = default)
	{
		var roundInfo = await roundRepository.GetAsync(
			roundId: query.RoundId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(query.RoundId.ToString(), $"This game bet does not exist.");
		var configuration = await gameProviderRepository.GetJokerConfigurationAsync(
			gameProviderConfigurationId: roundInfo.GameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await gameProviderRepository.GetGameProviderCultureCodeAsync(
			gameProviderId: roundInfo.GameProviderId,
			cultureCode: query.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "en";

		var url = await client.GameRecordDetailsAsync(
			apiUrl: configuration.ApiUrl,
			appId: configuration.AppId,
			secretKey: configuration.SecretKey,
			externalRoundId: roundInfo.ExternalRoundId,
			language: cultureCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new RoundDetailResult
		{
			Data = new RoundDetailDataBase[]
			{
				new RoundDetailDataUrl
				{
					Url = url??string.Empty
				}
			}
		};
	}
}
