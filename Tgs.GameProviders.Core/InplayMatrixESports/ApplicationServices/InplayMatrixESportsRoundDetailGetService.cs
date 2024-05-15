using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.InplayMatrixESports.ApplicationServices;

internal class InplayMatrixESportsRoundDetailGetService(
	IGameBetRepository gameBetRepository) : IGetService<InplayMatrixESportsRoundDetailQuery, RoundDetailResult>
{
	public async ValueTask<RoundDetailResult> GetAsync(InplayMatrixESportsRoundDetailQuery query, CancellationToken cancellationToken = default)
	{
		var gameBetInfo = await gameBetRepository.GetAsync(
			gameBetId: query.GameBetId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new RoundDetailResult()
		{
			Data = [new RoundDetailDataJson { Value = gameBetInfo.AdditionalData.ToJsonString() }]
		};
	}
}
