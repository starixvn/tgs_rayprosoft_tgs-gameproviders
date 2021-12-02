using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.Extensions;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace TGS.GameProviders.Core.Habanero.ApplicationServices
{
    internal class HabaneroRoundDetailGetService : IGetService<HabaneroRoundDetailQuery, RoundDetailResult>
    {
        private readonly ILogger<HabaneroRoundDetailGetService> _logger;
        private readonly IHabaneroService _service;
        private readonly IRoundRepository _roundRepository;

        public HabaneroRoundDetailGetService(
            ILogger<HabaneroRoundDetailGetService> logger,
            IHabaneroService service,
            IRoundRepository roundRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _roundRepository = roundRepository ?? throw new ArgumentNullException(nameof(roundRepository));
        }

        public async ValueTask<RoundDetailResult> GetAsync(
            HabaneroRoundDetailQuery query,
            CancellationToken cancellationToken = default)
        {
            var roundModel = await _roundRepository.GetAsync(query.RoundId).ConfigureAwait(false);
            if (roundModel == null)
            {
                throw new Exception($"This game round number could not be found.{query.RoundId}");
            }

            var gameProviderModel = await _service.GetGameProviderAsync(roundModel.GameProviderId).ConfigureAwait(false);
            if (gameProviderModel == null)
            {
                throw new Exception($"This game manufacturer does not exist.{roundModel.GameProviderId}");
            }

            var gameModel = await _service.GetGameAsync(roundModel.GameId).ConfigureAwait(false);
            if (gameModel == null)
            {
                throw new Exception($"This game Id could not be found.{roundModel.GameId}");
            }

            var playerModel = await _service.GetPlayerAsync(roundModel.PlayerCode).ConfigureAwait(false);
            if (playerModel == null)
            {
                throw new Exception($"This player could not be found.{roundModel.PlayerCode}");
            }

            var configuration = await _service
                .GetSettingConfigurationAsync()
                .ConfigureAwait(false);

            var gameProviderCulture = await _service
                .GetGameProviderLanguageCodeAsync(gameProviderModel.GameProviderId, query.Language)
                .ConfigureAwait(false);

            var valueToHash =
                $"{roundModel.ExternalRoundId.ToLower()}{configuration.BrandId.ToLower()}{configuration.ApiKey.ToLower()}";

            var hashKey = Sha256Hash(valueToHash);

            var url = $"{configuration.BetHistoryUrl.AbsoluteUri}games/history"
                .AppendQueryString($"brandId={configuration.BrandId}")
                .AppendQueryString($"friendlyid={roundModel.ExternalRoundId}")
                .AppendQueryString("viewtype=game")
                .AppendQueryString($"local={gameProviderCulture}")
                .AppendQueryString($"hash={hashKey}");

            return new RoundDetailResult
            {
                GameProviderName = gameProviderModel.Name,
                GameName = gameModel.Name,
                ExternalRoundId = roundModel.ExternalRoundId,
                BetAmount = roundModel.BetAmount,
                Turnover = roundModel.Turnover,
                Winloss = roundModel.WinLoss,
                GameResult = roundModel.ClientStatus.ToString(),
                Data = new RoundDetailDataBase[]
                {
                    new RoundDetailDataUrl
                    {
                        Url = url
                    }
                }
            };
        }

        private string Sha256Hash(string value)
            => new BigInteger(SHA256.HashData(Encoding.UTF8.GetBytes(value)), isBigEndian: true)
                .ToString("x");
    }
}