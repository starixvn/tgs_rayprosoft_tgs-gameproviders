using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core.Extensions;
using Tgs.GameProviders.Core.WGHorse.Models;
using Tgs.GameProviders.QueryServer.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WGHorse.ApplicationServices
{
    public class WGHorseGameRecordListGetService : IGetService<WGHorseGameRecordListQuery, GameRecordListResult>
    {
        private readonly IWGHorseApiClient _client;
        private readonly ILogger<WGHorseGameRecordListGetService> _logger;
        private readonly IWGHorseService _service;

        public WGHorseGameRecordListGetService(
            IWGHorseApiClient client,
            ILogger<WGHorseGameRecordListGetService> logger,
            IWGHorseService service
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<GameRecordListResult> GetAsync(WGHorseGameRecordListQuery command, CancellationToken cancellationToken = default)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            (bool Success, WGHorseGameRecord[]? GameRecord) = await _client.GetGameRecordAsync(
                configuration.ApiUrl,
                configuration.PartnerCode,
                configuration.PartnerKey,
                configuration.IV,
                command.StartTime,
                command.EndTime,
                command.Page,
                command.Rows
                ).ConfigureAwait(false);

            if (!Success)
            {
                return new() { Success = false };
            }
            if (Success && GameRecord == null)
            {
                return new() { Success = true };
            }

            var gameProviderCode = _service.GetGameProviderCode();
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);

            var datas = GameRecord.Select(data => new GameRecordListModel()
            {
                GameProviderCode = gameProviderCode,
                GameProviderId = gameProviderId,
                PlayerCode = data.MemberCode,
                Status = RoundStatus.Closed,
                ClientStatus = _service.GetClientRound(((data.ShowWin.IgnoreCaseContains("和局")) ? 0m : decimal.Parse(data.ShowWin))),
                ExternalRoundId = data.BatchNo,
                ExternalGameId = "1",
                ExternalCreatedOn = new DateTimeOffset(DateTime.Parse(data.RecordDate), TimeSpan.FromHours(+8)),
                ExternalClosedOn = new DateTimeOffset(DateTime.Parse(data.ShowDivideDate), TimeSpan.FromHours(+8)),
                BetAmount = decimal.Parse(data.ShowAmount),
                ValidBetAmount = decimal.Parse(data.ShowAmount),
                PlayerWinLoss = (data.ShowWin.IgnoreCaseContains("和局")) ? 0 : decimal.Parse(data.ShowWin),
                Commission = 0,
                Turnover = decimal.Parse(data.ShowAmount),
                SettleAmount = decimal.Parse(data.ShowAmount) + ((data.ShowWin.IgnoreCaseContains("和局")) ? 0m : decimal.Parse(data.ShowWin)),
                TieBetAdjustment = null,

                AdditionalData = JsonSerializer.Serialize(data),
            })
                .ToArray();
            return new() { Success = true, Datas = datas };
        }
    }
}
