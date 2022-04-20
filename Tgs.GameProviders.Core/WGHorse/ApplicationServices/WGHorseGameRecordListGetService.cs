using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core.WGHorse.Models;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.QueryServer.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WGHorse.ApplicationServices
{
    public class WGHorseGameRecordListGetService : IGetService<WGHorseGameRecordListQuery, GameRecordListResult>
    {
        private readonly IWGHorseApiClient _client;
        private readonly IWGHorseService _service;
        private readonly ILogger<WGHorseGameRecordListGetService> _logger;

        public WGHorseGameRecordListGetService(
            IWGHorseApiClient client,
            IWGHorseService service,
            ILogger<WGHorseGameRecordListGetService> logger
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

            var gameProviderCode = GameProviderType.WGHorse;
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);

            var datas = GameRecord
                .Select(data => new GameRecordListModel()
                {
                    GameProviderCode = gameProviderCode,
                    GameProviderId = gameProviderId,
                    PlayerCode = data.MemberCode,
                    Status = GetRoundStatus(data.ShowType, GetDateTimeOffsetUTC8(data.ShowDivideDate)),
                    ClientStatus = (GetRoundStatus(data.ShowType, GetDateTimeOffsetUTC8(data.ShowDivideDate)) == RoundStatus.Closed) ? _service.GetClientRound(IsDraw(data.ShowWin) ? 0m : decimal.Parse(data.ShowWin)) : RoundClientStatus.Open,
                    ExternalRoundId = data.BatchNo,
                    ExternalGameId = "1",
                    ExternalCreatedOn = GetDateTimeOffsetUTC8(data.RecordDate),
                    ExternalClosedOn = GetDateTimeOffsetUTC8(data.ShowDivideDate),
                    BetAmount = decimal.Parse(data.ShowAmount),
                    ValidBetAmount = decimal.Parse(data.ShowAmount),
                    PlayerWinLoss = (GetRoundStatus(data.ShowType, GetDateTimeOffsetUTC8(data.ShowDivideDate)) == RoundStatus.Closed) ? IsDraw(data.ShowWin) ? 0 : decimal.Parse(data.ShowWin) : 0,
                    Commission = 0,
                    Turnover = decimal.Parse(data.ShowAmount),
                    SettleAmount = (GetRoundStatus(data.ShowType, GetDateTimeOffsetUTC8(data.ShowDivideDate)) == RoundStatus.Closed) ? decimal.Parse(data.ShowAmount) + (IsDraw(data.ShowWin) ? 0m : decimal.Parse(data.ShowWin)) : 0,
                    TieBetAdjustment = null,
                    AdditionalData = JsonSerializer.Serialize(data),
                })
                .ToArray();
            return new() { Success = true, Datas = datas };
        }

        private RoundStatus GetRoundStatus(string showType, DateTimeOffset externalClosedOn)
        {
            _logger.LogInformation("LogOn:{DateTime.Now.ToString(\"o\")} NowOn:{DateTime.Parse(DateTimeOffset.Now.LocalDateTime.ToLongDateString())} externalClosedOn: { DateTime.Parse(externalClosedOn.LocalDateTime.ToLongDateString())}", DateTime.Now.ToString("o"), DateTime.Parse(DateTimeOffset.Now.LocalDateTime.ToLongDateString()), DateTime.Parse(externalClosedOn.LocalDateTime.ToLongDateString()));
            return showType.Contains("過關", StringComparison.OrdinalIgnoreCase) ? DateTime.Parse(DateTimeOffset.Now.LocalDateTime.ToLongDateString()) <= DateTime.Parse(externalClosedOn.LocalDateTime.ToLongDateString()) ? RoundStatus.Open : RoundStatus.Closed : RoundStatus.Closed;
        }

        private static bool IsDraw(string source)
            => source.Contains("和局", StringComparison.OrdinalIgnoreCase);

        private DateTimeOffset GetDateTimeOffsetUTC8(string dateTime)
            => new DateTimeOffset(DateTime.Parse(dateTime), TimeSpan.FromHours(+8));
    }
}
