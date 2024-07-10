using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.GTPlaytech.ApplicationServices;

internal class GTPlaytechRoundDetailGetService(
	IGameBetRepository gameBetRepository) : IGetService<GTPlaytechRoundDetailQuery, RoundDetailResult>
{
	public async ValueTask<RoundDetailResult> GetAsync(GTPlaytechRoundDetailQuery query, CancellationToken cancellationToken = default)
	{
		var gameBetInfo = await gameBetRepository.GetAsync(
			gameBetId: query.GameBetId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new RoundDetailResult()
		{
			Data = [new RoundDetailDataUrl { Url = gameBetInfo.AdditionalData["detailUrl"].ToString() ?? string.Empty }]
		};
	}
}
