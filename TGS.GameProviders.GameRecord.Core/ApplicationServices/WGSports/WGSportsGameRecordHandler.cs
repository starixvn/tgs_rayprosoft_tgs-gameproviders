using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.WGSports.ApplicationServices;
using TGS.GameProviders.GameRecord.Core.Processors;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Domains.Models.Game;
using TGS.GameProviders.Repositories.Domains.Models.Player;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration;

namespace TGS.GameProviders.GameRecord.Core.ApplicationServices.WGSports
{
    public class WGSportsGameRecordHandler : IGetService<WGSportsGameRecordHandlerQuery, WGSportsGameRecordHandlerResult>
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IGameRecordProcessor _gameRoundsProcessor;
        private readonly IGetService<WGSportsGameRecordListQuery, GameRecordListResult> _getService;
        private readonly IPlayerRepository _playerRepository;
        private readonly IWGSportsService _service;
        private WGSportsConfiguration _configuration;
        private GetGame _game;
        private Guid _gameProviderId;
        private Dictionary<string, (GetPlayer Player, Guid? GameProviderConfigurationId)> _players;

        public WGSportsGameRecordHandler(
            IWGSportsService service,
            IBrandRepository brandRepository,
            IPlayerRepository playerRepository,
            IGameRecordProcessor gameRoundsProcessor,
            IGetService<WGSportsGameRecordListQuery, GameRecordListResult> getService
            )
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _gameRoundsProcessor = gameRoundsProcessor ?? throw new ArgumentNullException(nameof(gameRoundsProcessor));
            _getService = getService ?? throw new ArgumentNullException(nameof(getService));
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

        private async ValueTask<(bool Success, int Count, int SuccessCount, int FailCount, DateTimeOffset? LastGameRecordTime)> ExecuteRecordAsync(GameRecordListModle[] datas)
        {
            var count = 0;
            var successCount = 0;
            var failCount = 0;
            var lastGameRecordTime = default(DateTimeOffset?);
            var sysDateTimeOffset = DateTime.UtcNow;
            foreach (var data in datas)
            {
                count++;
                var success = true;

                if (!_players.TryGetValue(data.PlayerCode, out (GetPlayer Player, Guid? GameProviderConfigurationId) playerInfo))
                {
                    var player = await _playerRepository.GetPlayerAsync(data.PlayerCode).ConfigureAwait(false);
                    var brandGameProviderConfigurationId = default(Guid?);
                    if (player != null)
                    {
                        brandGameProviderConfigurationId = await _brandRepository.GetBrandGameProviderConfigurationIdAsync(_gameProviderId, player.BrandId).ConfigureAwait(false);
                    }
                    playerInfo = new(player, brandGameProviderConfigurationId);
                    _players.Add(data.PlayerCode, playerInfo);
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

                await _gameRoundsProcessor.ExecuteAsync(new GameRoundsData()
                {
                    ExternalRoundId = data.ExternalRoundId,
                    GameProviderId = _gameProviderId,
                    GameProviderCode = _service.GetGameProviderCode(),
                    PlayerCode = (playerInfo.Player == null) ? string.Empty : playerInfo.Player.PlayerCode,
                    GameId = (_game == null) ? -1 : _game.GameId,
                    ExternalGameId = data.ExternalGameId,
                    CurrencyCode = (playerInfo.Player == null) ? string.Empty : playerInfo.Player.DefaultWalletCurrencyCode,
                    Status = data.Status,
                    GameProviderConfigurationId = playerInfo.GameProviderConfigurationId.GetValueOrDefault(),
                    WalletId = (playerInfo.Player == null) ? Guid.Empty : playerInfo.Player.DefaultWalletId,
                    CreatedOn = sysDateTimeOffset,
                    ClosedOn = data.ExternalClosedOn.HasValue ? sysDateTimeOffset : null,
                    UpdatedOn = sysDateTimeOffset,
                    BrandId = (playerInfo.Player == null) ? Guid.Empty : playerInfo.Player.BrandId,
                    ExternalCreatedOn = data.ExternalCreatedOn,
                    ExternalClosedOn = data.ExternalClosedOn,
                    BetAmount = data.BetAmount,
                    ValidBetAmount = data.ValidBetAmount,
                    PlayerWinLoss = data.PlayerWinLoss,
                    Commission = data.Commission,
                    Turnover = data.Turnover,
                    SettleAmount = data.SettleAmount,
                    TieBetAdjustment = data.TieBetAdjustment,
                    ClientStatus = data.ClientStatus,
                    AdditionalData = data.AdditionalData,
                    Success = success
                });
                lastGameRecordTime = data.ExternalClosedOn.HasValue ? data.ExternalClosedOn : data.ExternalCreatedOn;
            }

            return (true, count, successCount, failCount, lastGameRecordTime);
        }

        private async ValueTask<(bool Success, int Count, int SuccessCount, int FailCount, DateTimeOffset? LastGameRecordTime)> SettleAsync(WGSportsGameRecordHandlerQuery source)
        {
            var response = await _getService.GetAsync(
                new()
                {
                    IsSettlement = true,
                    StartTime = source.StartTime,
                    EndTime = source.EndTime
                }).ConfigureAwait(false);

            return !response.Success
                ? (false, 0, 0, 0, null)
                : response.Success && response?.Datas == null
                ? (true, 0, 0, 0, null)
                : await ExecuteRecordAsync(response?.Datas).ConfigureAwait(false);
        }

        private async ValueTask<(bool Success, int Count, int SuccessCount, int FailCount, DateTimeOffset? LastGameRecordTime)> UnSettleAsync(WGSportsGameRecordHandlerQuery source)
        {
            var response = await _getService.GetAsync(
                new()
                {
                    IsSettlement = false,
                    StartTime = source.StartTime,
                    EndTime = source.EndTime
                }).ConfigureAwait(false);

            return !response.Success
                ? (false, 0, 0, 0, null)
                : response.Success && response?.Datas == null
                ? (true, 0, 0, 0, null)
                : await ExecuteRecordAsync(response?.Datas).ConfigureAwait(false);
        }
    }
}
