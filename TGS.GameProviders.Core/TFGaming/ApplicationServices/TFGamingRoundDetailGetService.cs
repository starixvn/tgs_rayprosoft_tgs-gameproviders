using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.TFGaming.Models;
using TGS.GameProviders.Domains.RoundDetail;

namespace TGS.GameProviders.Core.TFGaming.ApplicationServices
{
    internal class TFGamingRoundDetailGetService : IGetService<TFGamingRoundDetailQuery, RoundDetailResult>
    {
        private readonly IRoundService _roundService;

        public TFGamingRoundDetailGetService(IRoundService roundService)
        {
            _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
        }

        public async ValueTask<RoundDetailResult> GetAsync(TFGamingRoundDetailQuery query, CancellationToken cancellationToken = default)
        {
            var roundInfo = await _roundService.GetAsync(query.RoundId).ConfigureAwait(false);
            if (roundInfo == null)
            {
                throw new Exception($"This game round number could not be found.{query.RoundId}");
            }

            var gameRecord = string.IsNullOrEmpty(roundInfo.AdditionalData)
                ? null
                : JsonSerializer.Deserialize<TFGameRecord>(roundInfo.AdditionalData, new JsonSerializerOptions
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
                    new RoundDetailDataJson {Value = JsonSerializer.Serialize(gameRecord)}
               }
               });
        }
    }
}
