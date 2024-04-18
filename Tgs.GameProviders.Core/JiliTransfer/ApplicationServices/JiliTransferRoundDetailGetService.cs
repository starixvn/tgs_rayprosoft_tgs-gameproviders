using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.Jili.Client;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.JiliTransfer.ApplicationServices;

internal class JiliTransferRoundDetailGetService(
	IRoundRepository roundRepository,
	IGameProviderRepository gameProviderRepository,
	IGameInterfaceJiliClient client) : IGetService<JiliTransferRoundDetailQuery, RoundDetailResult>
{
	public async ValueTask<RoundDetailResult> GetAsync(JiliTransferRoundDetailQuery query, CancellationToken cancellationToken = default)
	{
		var roundInfo = await roundRepository.GetAsync(
			roundId: query.RoundId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(query.RoundId.ToString(), $"This game bet does not exist.");
		var configuration = await gameProviderRepository.GetJiliTransferConfigurationAsync(
			gameProviderConfigurationId: roundInfo.GameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await gameProviderRepository.GetGameProviderCultureCodeAsync(
			gameProviderId: roundInfo.GameProviderConfigurationId,
			cultureCode: query.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "en-US";

		var result = await client.GetGameDetailUrlAsync(
			apiUrl: configuration.ApiUrl,
			agentId: configuration.AgentId,
			agentKey: configuration.AgentKey,
			wagersId: roundInfo.ExternalRoundId,
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
