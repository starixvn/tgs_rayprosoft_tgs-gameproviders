using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.LY.Models;
using TGS.GameProviders.Domains.RoundDetail;

namespace TGS.GameProviders.Core.LY.ApplicationServices
{
    internal class LYRoundDetailGetService : IGetService<LYRoundDetailQuery, RoundDetailResult>
    {
        private readonly IRoundService _roundService;

        public LYRoundDetailGetService(
            IRoundService roundService)
        {
            _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
        }

        public async ValueTask<RoundDetailResult> GetAsync(LYRoundDetailQuery query, CancellationToken cancellationToken = default)
        {
            var roundModel = await _roundService.GetAsync(query.RoundId).ConfigureAwait(false);
            if (roundModel == null)
            {
                throw new Exception($"This game round number could not be found.{query.RoundId}");
            }

            var gameRecord = JsonSerializer.Deserialize<LYGameRecord>(roundModel.AdditionalData, new JsonSerializerOptions
            {
                // 設定在做序列化/反序列化時，尊重名稱大小寫
                PropertyNameCaseInsensitive = true
            });

            return gameRecord == null
                ? (new()
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
                    new RoundDetailDataJson {Value = gameRecord.CardValue}
                }
                });
        }
    }
}
