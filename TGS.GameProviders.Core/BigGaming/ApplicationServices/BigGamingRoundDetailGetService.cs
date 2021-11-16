using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.BigGaming.Models;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.RoundDetail;
using System.Text.Json;

namespace TGS.GameProviders.Core.BigGaming.ApplicationServices
{
    internal class BigGamingRoundDetailGetService : IGetService<BigGamingRoundDetailQuery, RoundDetailResult>
    {
        private readonly IRoundRepository _roundRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IBigGamingService _service;
        private readonly IBigGamingApiClient _client;
        private readonly IBigGamingRoundDetailAnalyzeService _bigGamingRoundDetailAnalyzeService;
        public BigGamingRoundDetailGetService(
            IBigGamingApiClient client,
            IRoundRepository roundRepository,
            IGameRepository gameRepository,
            IBigGamingService service,
            IBigGamingRoundDetailAnalyzeService bigGamingRoundDetailAnalyzeService)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _roundRepository = roundRepository ?? throw new ArgumentNullException(nameof(roundRepository));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _bigGamingRoundDetailAnalyzeService = bigGamingRoundDetailAnalyzeService ?? throw new ArgumentNullException(nameof(bigGamingRoundDetailAnalyzeService));
        }
        public async ValueTask<RoundDetailResult> GetAsync(BigGamingRoundDetailQuery query, CancellationToken cancellationToken = default)
        {
            var round = await _roundRepository.GetRoundAsync(query.RoundId).ConfigureAwait(false);
            if (round == null)
            {
                throw new Exception($"This game round number could not be found.{query.RoundId}");
            }

            var game = await _gameRepository.GetGameAsync(round.GameId).ConfigureAwait(false);

            if (game == null)
            {
                throw new Exception($"This game Id could not be found..{round.GameId}");
            }

            var brandMerchant = await _service.GetBrandMerchantAsync(round.CurrencyCode).ConfigureAwait(false);


            if (game.Type == GameType.LiveDealer)
            {
                var request = new BigGamingVideoRoundQueryRequest
                {
                    Random = Guid.NewGuid().ToString(),
                    Sn = brandMerchant.SecretCode,
                    Round = round.ExternalRoundId
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
                    BetAmount = round.BetAmount ?? 0,
                    Turnover = round.Turnover ?? 0,
                    Winloss = round.WinLoss ?? 0,
                    GameResult = round.ClientStatus.ToString(),
                    Data = new RoundDetailDataBase[]
                    {
                    new RoundDetailDataJson {Value = value}
                    }
                };
            }
            else
            {
                var culture = await _service.GetGameProviderCultureCodeAsync(query.Language).ConfigureAwait(false);
                var record = JsonSerializer.Deserialize<BigGamingGameRecordFishingResponse>(round.AdditionalData);
                var request = new BigGamingGameRecordFishingUrlRequest
                {
                    Random = Guid.NewGuid().ToString(),
                    Sn = brandMerchant.SecretCode,
                    ExternalBetId = long.Parse(record.ExternalBetId),
                    GameId = int.Parse(game.GameExternalId),
                    Locale = culture
                };

                var response = await _client.GetGameRecordFishingUrlAsync(brandMerchant.ApiUrl, brandMerchant.SecretCode, brandMerchant.SecretKey, request).ConfigureAwait(false);

                return new RoundDetailResult
                {
                    BetAmount = round.BetAmount ?? 0,
                    Turnover = round.Turnover ?? 0,
                    Winloss = round.WinLoss ?? 0,
                    GameResult = round.ClientStatus.ToString(),
                    Data = new RoundDetailDataBase[]
                    {
                    new RoundDetailDataUrl {Url = JsonSerializer.Deserialize<string>(JsonSerializer.Serialize(response.Result))}
                    }
                };
            }
        }
    }
}
