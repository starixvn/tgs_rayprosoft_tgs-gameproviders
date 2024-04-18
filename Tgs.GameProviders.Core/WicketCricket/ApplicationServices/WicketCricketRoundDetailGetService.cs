using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WicketCricket.ApplicationServices;

internal class WicketCricketRoundDetailGetService(
	IRoundRepository roundRepository) : IGetService<WicketCricketRoundDetailQuery, RoundDetailResult>
{
	public async ValueTask<RoundDetailResult> GetAsync(WicketCricketRoundDetailQuery query, CancellationToken cancellationToken = default)
	{
		var roundInfo = await roundRepository.GetAsync(
			roundId: query.RoundId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(query.RoundId.ToString(), $"This game bet does not exist.");

		return new RoundDetailResult()
		{
			Data = new RoundDetailDataBase[]
			{
				new RoundDetailDataJson {Value = roundInfo.AdditionalData}
			}
		};
	}
}
