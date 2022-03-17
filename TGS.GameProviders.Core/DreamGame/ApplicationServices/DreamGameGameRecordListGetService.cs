using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.QueryServer.Models;
using Tgs.ServiceUtility;
using TGS.GameProviders.Domains;

namespace TGS.GameProviders.Core.DreamGame.ApplicationServices
{
    public class DreamGameGameRecordListGetService : IGetService<DreamGameGameRecordListQuery, GameRecordListResult>
    {
        private readonly IDreamGameApiClient _client;
        private readonly IDreamGameService _service;

        public DreamGameGameRecordListGetService(
            IDreamGameApiClient client,
            IDreamGameService service
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<GameRecordListResult> GetAsync(DreamGameGameRecordListQuery query, CancellationToken cancellationToken = default)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);


            var gameReports = await _client.GetReportAsync(configuration.ApiUrl, configuration.ApiKey, configuration.AgentName).ConfigureAwait(false);

            var gameProviderCode = GameProviderType.DreamGame; ;
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);

            var datas = gameReports.Select(data => new GameRecordListModel()
            {
                GameProviderCode = gameProviderCode,
                GameProviderId = gameProviderId,
                PlayerCode = data.UserName.ToLower(),
                Status = data.IsRevocation == 0 ? RoundStatus.Open : RoundStatus.Closed,
                ClientStatus = data.IsRevocation == 0 ? RoundClientStatus.Open : data.IsRevocation == 2 ? RoundClientStatus.Void : data.WinOrLoss == 0 ? RoundClientStatus.Draw : data.WinOrLoss > 0 ? RoundClientStatus.Win : RoundClientStatus.Loss,
                ExternalRoundId = data.Id.ToString(),
                ExternalGameId = data.TableId.ToString(),
                ExternalCreatedOn = new DateTimeOffset(DateTime.Parse(data.BetTime), TimeSpan.FromHours(+8)),
                ExternalClosedOn = data.IsRevocation != 0 ? new DateTimeOffset(DateTime.Parse(data.CalTime), TimeSpan.FromHours(+8)) : null,
                BetAmount = data.BetPoints,
                ValidBetAmount = data.AvailableBet,
                PlayerWinLoss = data.WinOrLoss - data.BetPoints,
                Commission = 0,
                Turnover = data.AvailableBet,
                SettleAmount = data.WinOrLoss,
                TieBetAdjustment = null,
                AdditionalData = JsonSerializer.Serialize(data),
            }).ToArray();

            return new() { Success = true, Datas = datas };
        }
    }
}
