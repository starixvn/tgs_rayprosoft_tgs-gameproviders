using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.ApiClients.BigGaming.Models;
using TGS.GameProviders.ApiClients.BigGaming.Models.Commons;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.BigGaming.ApplicationServices;
using TGS.GameProviders.Domains.Extensions;
using TGS.GameProviders.GameRecord.Core.Processors;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Domains;
using TGS.GameProviders.Repositories.Domains.Models.Game;
using TGS.GameProviders.Repositories.Domains.Models.Player;

namespace TGS.GameProviders.GameRecord.Core.ApplicationServices.BigGaming
{
    public class BigGamingFishingGameRecordHandler : IGetService<BigGamingFishingGameRecordHandlerQuery, BigGamingFishingGameRecordHandlerResult>
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IGetService<BigGamingFishingGameRecordListQuery, GameRecordListResult> _getService;
        private readonly IGameRepository _gameRepository;
        private readonly IGameRecordProcessor _gameRoundsProcessor;
        private readonly IPlayerRepository _playerRepository;
        private readonly IBigGamingService _service;

        public BigGamingFishingGameRecordHandler(
            IBigGamingService service,
            IGameRepository gameRepository,
            IBrandRepository brandRepository,
            IPlayerRepository playerRepository,
            IGameRecordProcessor gameRoundsProcessor,
            IGetService<BigGamingFishingGameRecordListQuery, GameRecordListResult> getService
            )
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _gameRoundsProcessor = gameRoundsProcessor ?? throw new ArgumentNullException(nameof(gameRoundsProcessor));
            _getService = getService ?? throw new ArgumentNullException(nameof(getService));
        }

        public async ValueTask<BigGamingFishingGameRecordHandlerResult> GetAsync(BigGamingFishingGameRecordHandlerQuery query, CancellationToken cancellationToken = default)
        {
            int pageIndex = 1;
            int pageSize = 500;
            var gameProviderId = await _service.GetGameProviderIdAsync();
            var players = new Dictionary<string, (GetPlayer Player, Guid? BrandGameProviderConfigurationIds)>();
            var games = new Dictionary<string, GetGame>();
            var count = 0;
            var successCount = 0;
            var failCount = 0;
            var lastGameRecordTime = default(DateTimeOffset?);
            var sysDateTimeOffset = DateTime.UtcNow;


            while (true)
            {
                var response = await _getService.GetAsync(new()
                {
                    CurrencyCode = query.CurrencyCode,
                    StartTime = query.StartTime,
                    EndTime = query.EndTime,
                    Page = pageIndex,
                    Rows = pageSize
                }).ConfigureAwait(false);
                if (!response.Success)
                {
                    return new() { Success = false };
                }

                if (response.Success && response.Datas == null)
                {
                    break;
                }

                pageIndex++;

                foreach (var data in response.Datas)
                {

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

                    if (!games.TryGetValue(data.ExternalGameId, out GetGame game))
                    {
                        game = await _gameRepository.GetGameAsync(gameProviderId, data.ExternalGameId).ConfigureAwait(false);
                        games.Add(data.ExternalGameId, game);
                    }

                    if (playerInfo.Player == null || game == null)
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
                        GameId = game != null ? game.GameId : -1,
                        ExternalGameId = data.ExternalGameId,
                        CurrencyCode = (playerInfo.Player == null) ? string.Empty : playerInfo.Player.DefaultWalletCurrencyCode,
                        Status = RoundStatus.Closed,
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

            return new()
            {
                Success = true,
                LastGameRecorTime = lastGameRecordTime,
                Count = count,
                SuccessCount = successCount,
                FailCount = failCount
            };
        }
    }
}
