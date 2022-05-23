using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.BetConstruct.ApplicationServices
{
    internal class BetConstructRoundDetailGetService : IGetService<BetConstructRoundDetailQuery, RoundDetailResult>
    {
        private readonly IRoundService _roundService;

        public BetConstructRoundDetailGetService(
            IRoundService roundService)
        {
            _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
        }

        public async ValueTask<RoundDetailResult> GetAsync(BetConstructRoundDetailQuery query, CancellationToken cancellationToken = default)
        {
            var roundInfo = await _roundService.GetAsync(query.RoundId).ConfigureAwait(false);
            if (roundInfo == null)
            {
                throw new Exception($"This game round number could not be found.{query.RoundId}");
            }

            var gameRecord = roundInfo.AdditionalData;

            return gameRecord == null
                ? (new RoundDetailResult()
                {
                    Data = new RoundDetailDataBase[]
                    {
                        new RoundDetailDataJson {Value = null}
                    }
                })
                : (new()
                {
                    Data = new RoundDetailDataBase[]
                {
                    new RoundDetailDataJson {Value = JsonSerializer.Serialize(gameRecord)}
                }
                });
        }
    }
}