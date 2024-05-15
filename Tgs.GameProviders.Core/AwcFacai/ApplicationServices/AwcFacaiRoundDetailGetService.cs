using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcFacai.ApplicationServices;

internal class AwcFacaiRoundDetailGetService(
	IGameBetRepository gameBetRepository) : IGetService<AwcFacaiRoundDetailQuery, RoundDetailResult>
{
	public async ValueTask<RoundDetailResult> GetAsync(AwcFacaiRoundDetailQuery query, CancellationToken cancellationToken = default)
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
