using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.WGHorse.Models;
using Tgs.GameProviders.Domains;
using Tgs.GameRounds.Common;
using Tgs.ServiceUtility;

#nullable enable

namespace Tgs.GameProviders.Core.WGHorse.ApplicationServices
{
    public class WGHorseGameRecordListGetService : IGetService<WGHorseGameRecordListQuery, GameRecordListResult>
    {
        private readonly IWGHorseApiClient _client;
        private readonly IWGHorseService _service;
        private readonly ISystemClock _systemClock;

        public WGHorseGameRecordListGetService(
            IWGHorseApiClient client,
            IWGHorseService service,
            ISystemClock systemClock
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
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
                    ClientStatus = IsClosed(data.ShowType, GetDateTimeOffsetUTC8(data.ShowDivideDate)) ? _service.GetClientRound(IsDraw(data.ShowWin) ? 0m : decimal.Parse(data.ShowWin)) : GameRoundClientStatus.Open,
                    ExternalRoundId = data.BatchNo,
                    ExternalGameId = "1",
                    ExternalCreatedOn = GetDateTimeOffsetUTC8(data.RecordDate),
                    ExternalClosedOn = GetDateTimeOffsetUTC8(data.ShowDivideDate),
                    BetAmount = decimal.Parse(data.ShowAmount),
                    ValidBetAmount = decimal.Parse(data.ShowAmount),
                    PlayerWinLoss = IsClosed(data.ShowType, GetDateTimeOffsetUTC8(data.ShowDivideDate)) ? IsDraw(data.ShowWin) ? 0 : decimal.Parse(data.ShowWin) : 0,
                    Commission = 0,
                    Turnover = decimal.Parse(data.ShowAmount),
                    SettleAmount = IsClosed(data.ShowType, GetDateTimeOffsetUTC8(data.ShowDivideDate)) ? decimal.Parse(data.ShowAmount) + (IsDraw(data.ShowWin) ? 0m : decimal.Parse(data.ShowWin)) : 0,
                    TieBetAdjustment = null,
                    AdditionalData = JsonSerializer.Serialize(data),
                })
                .ToArray();
            return new() { Success = true, Datas = datas };
        }

        private static DateTimeOffset GetDateTimeOffsetUTC8(string dateTime)
            => new DateTimeOffset(DateTime.Parse(dateTime), TimeSpan.FromHours(+8));

        private static DateTimeOffset GetTomorrowDateUTC8(DateTimeOffset dateTime)
            => new DateTimeOffset(DateTime.Parse(dateTime.AddDays(1).Date.ToLongDateString()), TimeSpan.FromHours(+8));

        private static bool IsDraw(string source)
            => source.Contains("和局", StringComparison.OrdinalIgnoreCase);

        private GameRoundStatus GetRoundStatus(string showType, DateTimeOffset externalClosedOn)
        {
            var tomorrowDateUTC8 = GetTomorrowDateUTC8(externalClosedOn);
            return showType.Contains("過關", StringComparison.OrdinalIgnoreCase) || showType.Contains("过关", StringComparison.OrdinalIgnoreCase) ? externalClosedOn.UtcDateTime < tomorrowDateUTC8.UtcDateTime && tomorrowDateUTC8.UtcDateTime <= _systemClock.CurrentTime.UtcDateTime ? GameRoundStatus.Closed : GameRoundStatus.Open : GameRoundStatus.Closed;
        }

        private bool IsClosed(string showType, DateTimeOffset externalClosedOn)
                    => GetRoundStatus(showType, externalClosedOn) == GameRoundStatus.Closed;
    }
}
