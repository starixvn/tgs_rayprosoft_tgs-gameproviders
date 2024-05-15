using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.JDBTransfer.ApplicationServices;

public class JDBTransferRoundDetailGetService(
	IGameBetRepository gameBetRepository) : IGetService<JDBTransferRoundDetailQuery, RoundDetailResult>
{
	public async ValueTask<RoundDetailResult> GetAsync(JDBTransferRoundDetailQuery query, CancellationToken cancellationToken = default)
	{
		var gameBetInfo = await gameBetRepository.GetAsync(
			gameBetId: query.GameBetId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new RoundDetailResult()
		{
			Data = [new RoundDetailDataJson { Value = gameBetInfo.AdditionalData.ToString() }]
		};
	}
}
