using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.JiliTransfer.ApplicationServices;

internal class JiliTransferRoundDetailGetService : IGetService<JiliTransferRoundDetailQuery, RoundDetailResult>
{
    private readonly IRoundService _roundService;

    public JiliTransferRoundDetailGetService(IRoundService roundService)
    {
        _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
    }

    public async ValueTask<RoundDetailResult> GetAsync(JiliTransferRoundDetailQuery query, CancellationToken cancellationToken = default)
    {
        var roundInfo = await _roundService.GetAsync(query.RoundId).ConfigureAwait(false);
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
