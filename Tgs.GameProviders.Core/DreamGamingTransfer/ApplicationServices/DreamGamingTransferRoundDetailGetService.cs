using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.DreamGamingService.Client;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.DreamGamingTransfer.ApplicationServices;

internal class DreamGamingTransferRoundDetailGetService(
	IGameBetRepository gameBetRepository,
	IGameProviderRepository gameProviderRepository,
	IGameInterfaceDreamGamingClient client) : IGetService<DreamGamingTransferRoundDetailQuery, RoundDetailResult>
{
	public async ValueTask<RoundDetailResult> GetAsync(DreamGamingTransferRoundDetailQuery query, CancellationToken cancellationToken = default)
	{
		var gameBetInfo = await gameBetRepository.GetAsync(
			gameBetId: query.GameBetId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var configuration = await gameProviderRepository.GetDreamGamingTransferConfigurationAsync(
			gameBetInfo.GameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var gameProviderCultureCode = await gameProviderRepository.GetGameProviderCultureCodeAsync(
			gameBetInfo.GameProviderId,
			query.Language,
			cancellationToken)
			.ConfigureAwait(false) ?? "cn";

		var reply = await client.GameHistoryDetailAsync(
			configuration.VideoGameApiUrl,
			configuration.AgentName,
			gameProviderCultureCode,
			gameBetInfo.ExternalBetId,
			cancellationToken)
			.ConfigureAwait(false);

		return new RoundDetailResult()
		{
			Data = [new RoundDetailDataUrl { Url = reply.GameResult }]
		};
	}
}
