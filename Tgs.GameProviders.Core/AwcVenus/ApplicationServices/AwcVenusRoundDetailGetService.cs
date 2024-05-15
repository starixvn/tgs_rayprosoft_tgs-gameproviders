using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcVenus.ApplicationServices;

internal class AwcVenusRoundDetailGetService(
	IGameBetRepository gameBetRepository) : IGetService<AwcVenusRoundDetailQuery, RoundDetailResult>
{
	public async ValueTask<RoundDetailResult> GetAsync(AwcVenusRoundDetailQuery query, CancellationToken cancellationToken = default)
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
