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
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGameService _gameService;
        private readonly IRoundService _roundService;

        public TFGamingRoundDetailGetService(
            IGameProviderRepository gameProviderRepository,
            IGameService gameService,
            IRoundService roundService)
        {
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
            _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
        }

        public async ValueTask<RoundDetailResult> GetAsync(TFGamingRoundDetailQuery query, CancellationToken cancellationToken = default)
        {
            var roundModel = await _roundService.GetAsync(query.RoundId).ConfigureAwait(false);
            if (roundModel == null)
            {
                throw new Exception($"This game round number could not be found.{query.RoundId}");
            }
            var gameProviderModel = await _gameProviderRepository.GetAsync(roundModel.GameProviderId).ConfigureAwait(false);
            if (gameProviderModel == null)
            {
                throw new Exception($"This game manufacturer does not exist.{roundModel.GameProviderId}");
            }

            var gameModel = await _gameService.GetCacheAsync(roundModel.GameId).ConfigureAwait(false);
            if (gameModel == null)
            {
                throw new Exception($"This game Id could not be found..{gameModel.GameId}");
            }

            var gameRecord = string.IsNullOrEmpty(roundModel.AdditionalData)
                ? null
                : JsonSerializer.Deserialize<TFGameRecord>(roundModel.AdditionalData, new JsonSerializerOptions
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
