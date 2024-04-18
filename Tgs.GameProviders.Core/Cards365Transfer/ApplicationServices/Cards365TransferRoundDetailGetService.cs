using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Cards365Transfer.ApplicationServices;

internal class Cards365TransferRoundDetailGetService(
	IRoundRepository roundRepository) : IGetService<Cards365TransferRoundDetailQuery, RoundDetailResult>
{
	public async ValueTask<RoundDetailResult> GetAsync(Cards365TransferRoundDetailQuery query, CancellationToken cancellationToken = default)
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
