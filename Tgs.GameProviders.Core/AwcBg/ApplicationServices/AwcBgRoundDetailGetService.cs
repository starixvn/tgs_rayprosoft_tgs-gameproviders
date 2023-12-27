using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcBg.ApplicationServices;

internal class AwcBgRoundDetailGetService(IRoundRepository round) : IGetService<AwcBgRoundDetailQuery, RoundDetailResult>
{
    public async ValueTask<RoundDetailResult> GetAsync(AwcBgRoundDetailQuery query, CancellationToken cancellationToken = default)
    {
        var roundInfo = await round.GetAsync(
            roundId: query.RoundId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return roundInfo == null
            ? throw new Exception($"This game round number could not be found.{query.RoundId}")
            : (new RoundDetailResult()
            {
                Data = new RoundDetailDataBase[]
                {
                    new RoundDetailDataJson {Value = roundInfo.AdditionalData}
                }
            });
    }
}
