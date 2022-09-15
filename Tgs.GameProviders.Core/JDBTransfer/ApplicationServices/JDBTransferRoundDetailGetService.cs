using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.JDBTransfer.ApplicationServices;

public class JDBTransferRoundDetailGetService : IGetService<JDBTransferRoundDetailQuery, RoundDetailResult>
{
    private readonly IRoundService _roundService;

    public JDBTransferRoundDetailGetService(IRoundService roundService)
    {
        _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
    }

    public async ValueTask<RoundDetailResult> GetAsync(JDBTransferRoundDetailQuery query, CancellationToken cancellationToken = default)
    {
        var roundInfo = await _roundService.GetAsync(query.RoundId).ConfigureAwait(false);
        if (roundInfo == null)
        {
            throw new Exception($"This game round number could not be found.{query.RoundId}");
        }

        return new()
        {
            Data = new RoundDetailDataBase[]
            {
                new RoundDetailDataJson { Value = roundInfo.AdditionalData }
            }
        };
    }
}
