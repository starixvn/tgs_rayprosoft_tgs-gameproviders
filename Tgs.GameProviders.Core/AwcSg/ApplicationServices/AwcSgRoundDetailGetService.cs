using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcSg.ApplicationServices;

internal class AwcSgRoundDetailGetService : IGetService<AwcSgRoundDetailQuery, RoundDetailResult>
{
    private readonly IRoundRepository _round;

    public AwcSgRoundDetailGetService(IRoundRepository round)
    {
        _round = round ?? throw new ArgumentNullException(nameof(round));
    }

    public async ValueTask<RoundDetailResult> GetAsync(AwcSgRoundDetailQuery query, CancellationToken cancellationToken = default)
    {
        var roundInfo = await _round.GetAsync(
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
