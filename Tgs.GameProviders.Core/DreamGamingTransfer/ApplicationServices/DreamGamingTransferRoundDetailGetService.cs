using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.DreamGamingService.Client;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.DreamGamingTransfer.ApplicationServices;

internal class DreamGamingTransferRoundDetailGetService(
	IRoundRepository roundRepository,
	IGameProviderRepository gameProviderRepository,
	IGameInterfaceDreamGamingClient client) : IGetService<DreamGamingTransferRoundDetailQuery, RoundDetailResult>
{
	public async ValueTask<RoundDetailResult> GetAsync(DreamGamingTransferRoundDetailQuery query, CancellationToken cancellationToken = default)
	{
		var roundInfo = await roundRepository.GetAsync(
			roundId: query.RoundId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(query.RoundId.ToString(), $"This game bet does not exist.");
		var configuration = await gameProviderRepository.GetDreamGamingTransferConfigurationAsync(
			roundInfo.GameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var gameProviderCultureCode = await gameProviderRepository.GetGameProviderCultureCodeAsync(
			roundInfo.GameProviderId,
			query.Language,
			cancellationToken)
			.ConfigureAwait(false) ?? "cn";

		var reply = await client.GameHistoryDetailAsync(
			configuration.VideoGameApiUrl,
			configuration.AgentName,
			gameProviderCultureCode,
			roundInfo.ExternalRoundId,
			cancellationToken)
			.ConfigureAwait(false);

		return new RoundDetailResult()
		{
			Data = new RoundDetailDataBase[]
				{
				new RoundDetailDataUrl { Url = reply.GameResult }
				}
		};
	}
}
