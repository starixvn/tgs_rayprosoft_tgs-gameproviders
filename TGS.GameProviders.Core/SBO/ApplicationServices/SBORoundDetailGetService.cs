using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.SBO.Models;
using TGS.GameProviders.Domains.RoundDetail;

namespace TGS.GameProviders.Core.SBO.ApplicationServices
{
    internal class SBORoundDetailGetService : IGetService<SBORoundDetailQuery, RoundDetailResult>
    {
        private readonly ISBOApiClient _client;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ILogger<SBORoundDetailGetService> _logger;
        private readonly IRoundRepository _roundRepository;
        private readonly ISBOService _service;
        private readonly ISBOUtility _utility;

        public SBORoundDetailGetService(
            ILogger<SBORoundDetailGetService> logger,
            IRoundRepository roundRepository,
            ISBOService service,
            ISBOUtility utility,
            ISBOApiClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _roundRepository = roundRepository ?? throw new ArgumentNullException(nameof(roundRepository));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
            _client = client ?? throw new ArgumentNullException(nameof(client));

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = false
            };
        }

        public async ValueTask<RoundDetailResult> GetAsync(SBORoundDetailQuery query,
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

            var gameProviderCultureCode = await _service
                .GetGameProviderCultureCodeAsync(roundModel.GameProviderId, query.Language)
                .ConfigureAwait(false);

            var configuration = await _service.GetSettingConfigurationAsync()
                .ConfigureAwait(false);

            var portfolio = _utility.GetPortfolio(gameModel.GameType);

            var request = new SBOGetRoundHistoryRequest
            {
                CompanyKey = configuration.CompanyKey,
                Portfolio = portfolio,
                ExternalRoundId = roundModel.ExternalRoundId,
                Language = gameProviderCultureCode
            };

            var response = await _client
                .GetRoundHistoryAsync(configuration.ApiUrl, request)
                .ConfigureAwait(false);

            var value = default(string);

            if (response.Error.Code == SBOApiErrorCode.None && response.Results?.Length > 0)
            {
                value = JsonSerializer.Serialize(response.Results, _jsonSerializerOptions);
            }

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
                    new RoundDetailDataJson
                    {
                        Value = value
                    }
                }
            };
        }
    }
}
