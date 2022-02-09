using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.BigGaming.Models;
using TGS.GameProviders.Domains.RoundDetail;

namespace TGS.GameProviders.Core.BigGaming.ApplicationServices
{
    internal class BigGamingRoundDetailGetService : IGetService<BigGamingRoundDetailQuery, RoundDetailResult>
    {
        private readonly IBigGamingRoundDetailAnalyzeService _bigGamingRoundDetailAnalyzeService;
        private readonly IBigGamingApiClient _client;
        private readonly IGameService _gameService;
        private readonly IRoundService _roundService;
        private readonly IBigGamingService _service;

        public BigGamingRoundDetailGetService(
            IBigGamingRoundDetailAnalyzeService bigGamingRoundDetailAnalyzeService,
            IBigGamingApiClient client,
            IGameService gameService,
            IRoundService roundService,
            IBigGamingService service
            )
        {
            _bigGamingRoundDetailAnalyzeService = bigGamingRoundDetailAnalyzeService ?? throw new ArgumentNullException(nameof(bigGamingRoundDetailAnalyzeService));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
            _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<RoundDetailResult> GetAsync(BigGamingRoundDetailQuery query, CancellationToken cancellationToken = default)
        {
            var roundModel = await _roundService.GetAsync(query.RoundId).ConfigureAwait(false);
            if (roundModel == null)
            {
                throw new Exception($"This game round number could not be found.{query.RoundId}");
            }

            var gameModel = await _gameService.GetCacheAsync(roundModel.GameId).ConfigureAwait(false);
            if (gameModel == null)
            {
                throw new Exception($"This game Id could not be found..{gameModel.GameId}");
            }

            var brandMerchant = await _service.GetBrandMerchantAsync(roundModel.CurrencyCode).ConfigureAwait(false);

            if (gameModel.GameType == GameType.LiveDealer)
            {
                var request = new BigGamingVideoRoundQueryRequest
                {
                    Random = Guid.NewGuid().ToString(),
                    Sn = brandMerchant.SecretCode,
                    Round = roundModel.ExternalRoundId
                };

                var response = await _client.GetVideoRoundQueryAsync(brandMerchant.ApiUrl, brandMerchant.SecretKey, request).ConfigureAwait(false);

                var value = default(string);
                if (response.Error == null)
                {
                    var gamingVideoRoundQueryResponse = JsonSerializer.Deserialize<BigGamingVideoRoundQueryResponse>(JsonSerializer.Serialize(response.Result));
                    if (gamingVideoRoundQueryResponse != null)
                    {
                        value = _bigGamingRoundDetailAnalyzeService.GetAnalyze(gamingVideoRoundQueryResponse.Items.First());
                    }
                }

                return new RoundDetailResult
                {
                    Data = new RoundDetailDataBase[]
                    {
                    new RoundDetailDataJson {Value = value}
                    }
                };
            }
            else
            {
                var culture = await _service.GetGameProviderCultureCodeAsync(query.Language).ConfigureAwait(false);
                var record = JsonSerializer.Deserialize<BigGamingGameRecordFishingResponse>(roundModel.AdditionalData);
                var request = new BigGamingGameRecordFishingUrlRequest
                {
                    Random = Guid.NewGuid().ToString(),
                    Sn = brandMerchant.SecretCode,
                    ExternalBetId = long.Parse(record.ExternalBetId),
                    GameId = int.Parse(gameModel.GameExternalId),
                    Locale = culture
                };

                var response = await _client.GetGameRecordFishingUrlAsync(brandMerchant.ApiUrl, brandMerchant.SecretCode, brandMerchant.SecretKey, request).ConfigureAwait(false);

                return new RoundDetailResult
                {
                    Data = new RoundDetailDataBase[]
                    {
                    new RoundDetailDataUrl {Url = JsonSerializer.Deserialize<string>(JsonSerializer.Serialize(response.Result))}
                    }
                };
            }
        }
    }
}
