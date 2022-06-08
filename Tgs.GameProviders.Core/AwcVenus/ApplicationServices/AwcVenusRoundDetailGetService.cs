using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcVenus.ApplicationServices
{
    internal class AwcVenusRoundDetailGetService : IGetService<AwcVenusRoundDetailQuery, RoundDetailResult>
    {
        private readonly IRoundService _roundService;

        public AwcVenusRoundDetailGetService(IRoundService roundService)
        {
            _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
        }

        public async ValueTask<RoundDetailResult> GetAsync(AwcVenusRoundDetailQuery query, CancellationToken cancellationToken = default)
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
}
