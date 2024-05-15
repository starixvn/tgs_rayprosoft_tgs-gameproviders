using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcCockfight.ApplicationServices;

internal class AwcCockfightRoundDetailGetService(
	IGameBetRepository gameBetRepository) : IGetService<AwcCockfightRoundDetailQuery, RoundDetailResult>
{
	public async ValueTask<RoundDetailResult> GetAsync(AwcCockfightRoundDetailQuery query, CancellationToken cancellationToken = default)
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
