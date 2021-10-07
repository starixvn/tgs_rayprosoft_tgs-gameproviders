using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TGS.GameProviders.ApiClients.WGSports;
using TGS.GameProviders.ApiClients.WGSports.Models;
using TGS.GameProviders.ApiClients.WGSports.Models.Commons;
using TGS.GameProviders.Core;
using TGS.GameProviders.Domains.Extensions;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.GameRecordProcessors;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Domains;
using TGS.GameProviders.Repositories.Domains.Models.Game;
using TGS.GameProviders.Repositories.Domains.Models.Player;
using TGS.GameProviders.Repositories.Entities;
using TGS.ServiceUtility;

namespace TGS.GameProviders.GameRecordReceiver.ApplicationServices.WGSports
{
    public class WGSportsGameRecordHandler : IGetService<WGSportsGameRecordHandlerQuery, WGSportsGameRecordHandlerResult>
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IWGSportsApiClient _client;
        private readonly IGameRepository _gameRepository;
        private readonly IGameRecordProcessor _gameRoundsProcessor;
        private readonly IPlayerRepository _playerRepository;
        private readonly IWGSportsService _service;
        private WGSportsConfiguration _configuration;
        private GetGame _game;
        private Guid _gameProviderId;
        private Dictionary<string, (GetPlayer Player, Guid? GameProviderConfigurationId)> _players;

        public WGSportsGameRecordHandler(
            IWGSportsApiClient client,
            IWGSportsService service,
            IGameRepository gameRepository,
            IBrandRepository brandRepository,
            IPlayerRepository playerRepository,
            IGameRecordProcessor gameRoundsProcessor
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _gameRoundsProcessor = gameRoundsProcessor ?? throw new ArgumentNullException(nameof(gameRoundsProcessor));
        }

        public async ValueTask<WGSportsGameRecordHandlerResult> GetAsync(WGSportsGameRecordHandlerQuery query, CancellationToken cancellationToken = default)
        {
            _configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
            _gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
            _players = new Dictionary<string, (GetPlayer Player, Guid? GameProviderConfigurationId)>();
            _game = await _service.GetGameByExternalGameId(_gameProviderId, "1").ConfigureAwait(false);

            var responseUnSettle = await UnSettleAsync(query).ConfigureAwait(false);
            if (!responseUnSettle.Success)
            {
                return new()
                {
                    Success = false
                };
            }

            var responseSettle = await SettleAsync(query).ConfigureAwait(false);
            if (!responseSettle.Success)
            {
                return new()
                {
                    Success = false
                };
            }

            var LastGameRecorTime = responseUnSettle.LastGameRecordTime;
            if (LastGameRecorTime == null)
            {
                LastGameRecorTime = responseSettle.LastGameRecordTime;
            }
            else
            {
                if (responseSettle.LastGameRecordTime != null)
                {
                    LastGameRecorTime = (LastGameRecorTime < responseSettle.LastGameRecordTime) ? responseSettle.LastGameRecordTime : LastGameRecorTime;
                }
            }

            return new WGSportsGameRecordHandlerResult()
            {
                Success = true,
                Count = responseUnSettle.Count + responseSettle.Count,
                SuccessCount = responseUnSettle.SuccessCount + responseSettle.SuccessCount,
                FailCount = responseUnSettle.FailCount + responseSettle.FailCount,
                LastGameRecorTime = LastGameRecorTime
            };
        }

        private async ValueTask<(bool Success, int Count, int SuccessCount, int FailCount, DateTimeOffset? LastGameRecordTime)> ExecuteRecordAsync(WGSportsBetInfo[] betInfos)
        {
            var count = 0;
            var successCount = 0;
            var failCount = 0;
            var lastGameRecordTime = default(DateTimeOffset?);
            var sysDateTimeOffset = DateTime.UtcNow;
            for (int i = 0; i < betInfos.Length; i++)
            {
                count++;
                var success = true;
                var gameRecord = betInfos[i];

                if (!_players.TryGetValue(gameRecord.Username, out (GetPlayer Player, Guid? GameProviderConfigurationId) playerInfo))
                {
                    var player = await _playerRepository.GetPlayerAsync(gameRecord.Username).ConfigureAwait(false);
                    var brandGameProviderConfigurationId = default(Guid?);
                    if (player != null)
                    {
                        brandGameProviderConfigurationId = await _brandRepository.GetBrandGameProviderConfigurationIdAsync(_gameProviderId, player.BrandId).ConfigureAwait(false);
                    }
                    playerInfo = new(player, brandGameProviderConfigurationId);
                    _players.Add(gameRecord.Username, playerInfo);
                }

                if (playerInfo.Player == null || _game == null)
                {
                    success = false;
                    failCount++;
                }
                else
                {
                    successCount++;
                }

                if (gameRecord.Reference.IgnoreCaseContains("不接受"))
                {
                    continue;
                }

                var createdOn = new DateTimeOffset(DateTime.Parse(gameRecord.RecordDate), TimeSpan.FromHours(-4));
                DateTimeOffset? externalClosedOn = gameRecord.InitData.Win.IgnoreCaseEquals("0") ? null : new DateTimeOffset(DateTime.Parse(gameRecord.SettleDate), TimeSpan.FromHours(-4));
                await _gameRoundsProcessor.ExecuteAsync(new GameRoundsData()
                {
                    ExternalRoundId = gameRecord.Reference,
                    GameProviderId = _gameProviderId,
                    GameProviderCode = _service.GetGameProviderCode(),
                    PlayerCode = (playerInfo.Player == null) ? string.Empty : playerInfo.Player.PlayerCode,
                    GameId = (_game == null) ? -1 : _game.GameId,
                    ExternalId = gameRecord.Reference.ToString(),
                    CurrencyCode = (playerInfo.Player == null) ? string.Empty : playerInfo.Player.DefaultWalletCurrencyCode,
                    Status = gameRecord.InitData.Win.IgnoreCaseEquals("0") ? RoundStatus.Open : RoundStatus.Closed,
                    GameProviderConfigurationId = playerInfo.GameProviderConfigurationId.GetValueOrDefault(),
                    WalletId = (playerInfo.Player == null) ? Guid.Empty : playerInfo.Player.DefaultWalletId,
                    CreatedOn = sysDateTimeOffset,
                    ClosedOn = gameRecord.InitData.Win.IgnoreCaseEquals("0") ? null : sysDateTimeOffset,
                    UpdatedOn = sysDateTimeOffset,
                    BrandId = (playerInfo.Player == null) ? Guid.Empty : playerInfo.Player.BrandId,
                    ExternalCreatedOn = createdOn,
                    ExternalClosedOn = externalClosedOn,
                    BetAmount = decimal.Parse(gameRecord.InitData.BetAmount),
                    ValidBetAmount = decimal.Parse(gameRecord.InitData.BetAmount),
                    PlayerWinLoss = gameRecord.InitData.Win.IgnoreCaseEquals("0") ? null : decimal.Parse(gameRecord.Result),
                    Commission = 0,
                    Turnover = gameRecord.InitData.Win.IgnoreCaseEquals("0") ? null : decimal.Parse(gameRecord.InitData.BetAmount),
                    SettleAmount = decimal.Parse(gameRecord.InitData.BetAmount) + decimal.Parse(gameRecord.Result),
                    TieBetAdjustment = null,
                    ClientStatus = gameRecord.InitData.Win.IgnoreCaseEquals("0") ? ClientRoundStatus.Open : _service.GetClientRound(decimal.Parse(gameRecord.Result)),
                    AdditionalData = JsonSerializer.Serialize(gameRecord),
                    Success = success
                });
                lastGameRecordTime = (externalClosedOn.HasValue) ? externalClosedOn : createdOn;
            }

            return (true, count, successCount, failCount, lastGameRecordTime);
        }

        private async ValueTask<(bool Success, int Count, int SuccessCount, int FailCount, DateTimeOffset? LastGameRecordTime)> SettleAsync(WGSportsGameRecordHandlerQuery source)
        {
            var request = new WGSportsGameRecordRequest()
            {
                GameRecordTime = GameRecordTime.SettleDate,
                StartTime = source.StartTime.UtcDateTime.AddHours(-4).ToString("yyyy-MM-dd HH:mm:ss"),
                EndTime = source.EndTime.UtcDateTime.AddHours(-4).ToString("yyyy-MM-dd HH:mm:ss")
            };

            var response = await _client.GetGameRecordAsync(_configuration, request).ConfigureAwait(false);
            if (response.ReturnCode.IgnoreCaseNotEquals("1"))
            {
                return (false, 0, 0, 0, null);
            }
            if (response.BetInfo == null || response.BetInfo.Length == 0)
            {
                return (true, 0, 0, 0, null);
            }
            return await ExecuteRecordAsync(response.BetInfo).ConfigureAwait(false);
        }

        private async ValueTask<(bool Success, int Count, int SuccessCount, int FailCount, DateTimeOffset? LastGameRecordTime)> UnSettleAsync(WGSportsGameRecordHandlerQuery source)
        {
            var request = new WGSportsGameRecordRequest()
            {
                GameRecordTime = GameRecordTime.BetDate,
                StartTime = source.StartTime.UtcDateTime.AddHours(-4).ToString("yyyy-MM-dd HH:mm:ss"),
                EndTime = source.EndTime.UtcDateTime.AddHours(-4).ToString("yyyy-MM-dd HH:mm:ss")
            };

            var response = await _client.GetGameRecordAsync(_configuration, request).ConfigureAwait(false);
            if (response.ReturnCode.IgnoreCaseNotEquals("1"))
            {
                return (false, 0, 0, 0, null);
            }
            if (response.BetInfo == null || response.BetInfo.Length == 0)
            {
                return (true, 0, 0, 0, null);
            }
            return await ExecuteRecordAsync(response.BetInfo).ConfigureAwait(false);
        }
    }
}
