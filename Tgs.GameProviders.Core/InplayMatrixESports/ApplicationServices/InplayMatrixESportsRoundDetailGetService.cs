using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.InplayMatrixESports.ApplicationServices;

internal class InplayMatrixESportsRoundDetailGetService : IGetService<InplayMatrixESportsRoundDetailQuery, RoundDetailResult>
{
    private readonly IRoundRepository _roundService;

    public InplayMatrixESportsRoundDetailGetService(
        IRoundRepository roundService)
    {
        _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
    }

    public async ValueTask<RoundDetailResult> GetAsync(InplayMatrixESportsRoundDetailQuery query, CancellationToken cancellationToken = default)
    {
        var roundInfo = await _roundService
            .GetAsync(
            roundId: query.RoundId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new RoundDetailResult()
        {
            Data = new RoundDetailDataBase[]
            {
                new RoundDetailDataJson {Value = roundInfo.AdditionalData}
            }
        };
    }
}
