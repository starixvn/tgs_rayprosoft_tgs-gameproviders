using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcLudo.ApplicationServices;

internal class AwcLudoRoundDetailGetService : IGetService<AwcLudoRoundDetailQuery, RoundDetailResult>
{
    private readonly IRoundRepository _round;

    public AwcLudoRoundDetailGetService(IRoundRepository round)
    {
        _round = round ?? throw new ArgumentNullException(nameof(round));
    }

    public async ValueTask<RoundDetailResult> GetAsync(AwcLudoRoundDetailQuery query, CancellationToken cancellationToken = default)
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
