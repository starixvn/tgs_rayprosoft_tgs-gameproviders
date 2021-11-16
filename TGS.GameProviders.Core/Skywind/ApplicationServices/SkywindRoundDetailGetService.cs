using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Skywind.Models;
using TGS.GameProviders.Domains.RoundDetail;

namespace TGS.GameProviders.Core.Skywind.ApplicationServices
{
    internal class SkywindRoundDetailGetService : IGetService<SkywindRoundDetailQuery, RoundDetailResult>
    {
        private readonly ISkywindApiClient _client;
        private readonly IRoundRepository _roundRepository;
        private readonly ISkywindService _service;
        private readonly ISkywindAccessToken _skywindAccessToken;

        public SkywindRoundDetailGetService(
            ISkywindApiClient client,
            IRoundRepository roundRepository,
            ISkywindService service,
            ISkywindAccessToken skywindAccessToken
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _roundRepository = roundRepository ?? throw new ArgumentNullException(nameof(roundRepository));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _skywindAccessToken = skywindAccessToken ?? throw new ArgumentNullException(nameof(skywindAccessToken));
        }

        public async ValueTask<RoundDetailResult> GetAsync(SkywindRoundDetailQuery query, CancellationToken cancellationToken = default)
        {
            var round = await _roundRepository.GetRoundAsync(query.RoundId).ConfigureAwait(false);
            if (round == null)
            {
                throw new Exception($"This game round number could not be found.{query.RoundId}");
            }

            var roundId = round.ExternalRoundId;
            if (roundId.IndexOf("_") != -1)
            {
                roundId = roundId.Split("_")[0];
            }

            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            var getaccessToken = await _skywindAccessToken.GetAccessTokenAsync().ConfigureAwait(false);
            var response = await _client
                .GetGamHistoryRoundDetailsAsync(
                configuration.ApiUrl,
                getaccessToken,
                new SkywindGamHistoryRoundDetailsRequest
                {
                    RoundId = roundId
                }).ConfigureAwait(false);

            return new()
            {
                BetAmount = round.BetAmount ?? 0,
                Turnover = round.Turnover ?? 0,
                Winloss = round.WinLoss ?? 0,
                GameResult = round.ClientStatus.ToString(),
                Data = new RoundDetailDataBase[]
                {
                    new RoundDetailDataUrl {Url = response.ImageUrl}
                }
            };
        }
    }
}
