using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.V8Poker.ApplicationServices;

internal class V8PokerRoundDetailGetService(
	IRoundRepository roundRepository) : IGetService<V8PokerRoundDetailQuery, RoundDetailResult>
{
	public async ValueTask<RoundDetailResult> GetAsync(V8PokerRoundDetailQuery query, CancellationToken cancellationToken = default)
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
