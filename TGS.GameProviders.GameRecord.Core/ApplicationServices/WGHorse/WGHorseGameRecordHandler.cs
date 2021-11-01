using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.WGHorse.ApplicationServices;
using TGS.GameProviders.GameRecord.Core.Processors;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Domains.Models.Player;

namespace TGS.GameProviders.GameRecord.Core.ApplicationServices.WGHorse
{
    public class WGHorseGameRecordHandler : IGetService<WGHorseGameRecordHandlerQuery, WGHorseGameRecordHandlerResult>
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IGameRecordProcessor _gameRoundsProcessor;
        private readonly IGetService<WGHorseGameRecordListQuery, GameRecordListResult> _getService;
        private readonly IPlayerRepository _playerRepository;
        private readonly IWGHorseService _service;

        public WGHorseGameRecordHandler(
            IWGHorseService service,
            IBrandRepository brandRepository,
            IPlayerRepository playerRepository,
            IGameRecordProcessor gameRoundsProcessor,
            IGetService<WGHorseGameRecordListQuery, GameRecordListResult> getService
            )
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _gameRoundsProcessor = gameRoundsProcessor ?? throw new ArgumentNullException(nameof(gameRoundsProcessor));
            _getService = getService ?? throw new ArgumentNullException(nameof(getService));
        }

        public async ValueTask<WGHorseGameRecordHandlerResult> GetAsync(WGHorseGameRecordHandlerQuery query, CancellationToken cancellationToken = default)
        {
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
            var players = new Dictionary<string, (GetPlayer Player, Guid? GameProviderConfigurationId)>();
            var game = await _service.GetGameByExternalGameId(gameProviderId, "1").ConfigureAwait(false);
            var count = 0;
            var successCount = 0;
            var failCount = 0;
            var lastGameRecordTime = default(DateTimeOffset?);
            var page = 1;
            var rows = 1000;

            while (true)
            {
                var gameRecordListQuery = new WGHorseGameRecordListQuery
                {
                    StartTime = query.StartTime,
                    EndTime = query.EndTime,
                    Page = page,
                    Rows = rows
                };

                var response = await _getService.GetAsync(gameRecordListQuery).ConfigureAwait(false);

                if (!response.Success)
                {
                    return new() { Success = response.Success };
                }

                if (response.Success && response.Datas == null)
                {
                    break;
                }

                page++;

                var sysDateTimeOffset = DateTimeOffset.UtcNow;

                foreach (var data in response.Datas)
                {
                    count++;
                    var success = true;
                    if (!players.TryGetValue(data.PlayerCode, out (GetPlayer Player, Guid? GameProviderConfigurationId) playerInfo))
                    {
                        var player = await _playerRepository.GetPlayerAsync(data.PlayerCode).ConfigureAwait(false);
                        var brandGameProviderConfigurationId = default(Guid?);
                        if (player != null)
                        {
                            brandGameProviderConfigurationId = await _brandRepository.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
                        }
                        playerInfo = new(player, brandGameProviderConfigurationId);
                        players.Add(data.PlayerCode, playerInfo);
                    }

                    success = true;
                    if (playerInfo.Player == null)
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
                        GameProviderId = gameProviderId,
                        GameProviderCode = _service.GetGameProviderCode(),
                        PlayerCode = (playerInfo.Player == null) ? string.Empty : playerInfo.Player.PlayerCode,
                        GameId = (game == null) ? -1 : game.GameId,
                        ExternalGameId = data.ExternalGameId,
                        CurrencyCode = (playerInfo.Player == null) ? string.Empty : playerInfo.Player.DefaultWalletCurrencyCode,
                        Status = data.Status,
                        GameProviderConfigurationId = playerInfo.GameProviderConfigurationId.GetValueOrDefault(),
                        WalletId = (playerInfo.Player == null) ? Guid.Empty : playerInfo.Player.DefaultWalletId,
                        CreatedOn = sysDateTimeOffset,
                        ClosedOn = sysDateTimeOffset,
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
                    lastGameRecordTime = data.ExternalClosedOn;
                }
            }
            return new WGHorseGameRecordHandlerResult()
            {
                Success = true,
                Count = count,
                SuccessCount = successCount,
                FailCount = failCount,
                LastGameRecorTime = lastGameRecordTime
            };
        }
    }
}
