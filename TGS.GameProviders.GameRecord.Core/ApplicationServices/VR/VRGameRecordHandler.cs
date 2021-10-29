using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.VR.ApplicationServices;
using TGS.GameProviders.GameRecord.Core.Processors;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Domains;
using TGS.GameProviders.Repositories.Domains.Models.Game;
using TGS.GameProviders.Repositories.Domains.Models.Player;

namespace TGS.GameProviders.GameRecord.Core.ApplicationServices.VR
{
    public class VRGameRecordHandler : IGetService<VRGameRecordHandlerQuery, VRGameRecordHandlerResult>
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IGameRecordProcessor _gameRoundsProcessor;
        private readonly IGetService<VRGameRecordListQuery, VRGameRecordListResult> _getService;
        private readonly IPlayerRepository _playerRepository;
        private readonly IVRService _service;
        private int _count;
        private int _failCount;
        private Dictionary<int, GetGame> _games;
        private DateTimeOffset? _lastGameRecordTime;
        private Dictionary<string, (GetPlayer Player, Guid? GameProviderConfigurationId)> _players;
        private int _successCount;

        public VRGameRecordHandler(
            IVRService service,
            IGameRepository gameRepository,
            IBrandRepository brandRepository,
            IPlayerRepository playerRepository,
            IGameRecordProcessor gameRoundsProcessor,
            IGetService<VRGameRecordListQuery, VRGameRecordListResult> getService
            )
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _gameRoundsProcessor = gameRoundsProcessor ?? throw new ArgumentNullException(nameof(gameRoundsProcessor));
            _getService = getService ?? throw new ArgumentNullException(nameof(getService));
        }

        public async ValueTask<VRGameRecordHandlerResult> GetAsync(VRGameRecordHandlerQuery query, CancellationToken cancellationToken = default)
        {
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);

            _players = new Dictionary<string, (GetPlayer Player, Guid? GameProviderConfigurationId)>();
            _games = new Dictionary<int, GetGame>();
            _count = 0;
            _successCount = 0;
            _failCount = 0;
            _lastGameRecordTime = default;
            var unsettled = await GetGameRoundAsync(query, query.BrandCode, gameProviderId, false);
            if (!unsettled)
            {
                return new VRGameRecordHandlerResult
                {
                    Success = false,
                    SuccessCount = _successCount,
                    FailCount = _failCount,
                    Count = _count
                };
            }

            var settled = await GetGameRoundAsync(query, query.BrandCode, gameProviderId, true);
            if (!settled)
            {
                return new VRGameRecordHandlerResult
                {
                    Success = false,
                    LastGameRecorTime = _lastGameRecordTime,
                    SuccessCount = _successCount,
                    FailCount = _failCount,
                    Count = _count
                };
            }

            return new()
            {
                Success = true,
                LastGameRecorTime = _lastGameRecordTime,
                SuccessCount = _successCount,
                FailCount = _failCount,
                Count = _count
            };
        }

        /// <summary>
        /// Gets the game round asynchronous.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="brandCode">The brand code.</param>
        /// <param name="gameProviderId">The game provider identifier.</param>
        /// <param name="isUpdateTime">if set to <c>true</c> [is update time].</param>
        /// <returns></returns>
        private async Task<bool> GetGameRoundAsync(VRGameRecordHandlerQuery query, string brandCode, Guid gameProviderId, bool isUpdateTime)
        {
            var page = 0;
            var rows = 500;
            while (true)
            {
                var data = new VRGameRecordListQuery()
                {
                    BrandCode = brandCode,
                    StartTime = query.StartTime,
                    EndTime = query.EndTime,
                    IsUpdateTime = isUpdateTime,
                    Page = page,
                    Rows = rows
                };

                var response = await _getService.GetAsync(data);

                if (!response.Success)
                {
                    return false;
                }
                if (response.Success && response.Datas == null)
                {
                    return true;
                }

                page++;

                var sysDateTimeOffset = DateTimeOffset.UtcNow;

                foreach (var recordBet in response.Datas)
                {
                    _count++;
                    var success = true;
                    var playerCode = recordBet.PlayerName;

                    if (isUpdateTime)
                    {
                        _lastGameRecordTime = recordBet.UpdateTime;
                    }

                    if (!_players.TryGetValue(playerCode, out (GetPlayer Player, Guid? GameProviderConfigurationId) playerInfo))
                    {
                        var player = await _playerRepository.GetPlayerAsync(playerCode).ConfigureAwait(false);
                        var brandGameProviderConfigurationId = default(Guid?);
                        if (player != null)
                        {
                            brandGameProviderConfigurationId = await _brandRepository.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
                        }
                        playerInfo = new(player, brandGameProviderConfigurationId);
                        _players.Add(playerCode, playerInfo);
                    }

                    if (!_games.TryGetValue(recordBet.ChannelId, out GetGame game))
                    {
                        game = await _gameRepository.GetGameAsync(gameProviderId, recordBet.ChannelId.ToString()).ConfigureAwait(false);
                        _games.Add(recordBet.ChannelId, game);
                    }

                    if (playerInfo.Player == null || game == null)
                    {
                        success = false;
                        _failCount++;
                    }
                    else
                    {
                        _successCount++;
                    }

                    await _gameRoundsProcessor.ExecuteAsync(new GameRoundsData
                    {
                        ExternalRoundId = recordBet.SerialNumber,
                        GameProviderId = gameProviderId,
                        GameProviderCode = _service.GetGameProviderCode(),
                        PlayerCode = (playerInfo.Player == null) ? string.Empty : playerInfo.Player.PlayerCode,
                        GameId = (game == null) ? -1 : game.GameId,
                        ExternalId = recordBet.ChannelId.ToString(),
                        CurrencyCode = (playerInfo.Player == null) ? string.Empty : playerInfo.Player.DefaultWalletCurrencyCode,
                        Status = _service.GetRoundStatus(recordBet.State),
                        GameProviderConfigurationId = playerInfo.GameProviderConfigurationId.GetValueOrDefault(),
                        WalletId = (playerInfo.Player == null) ? Guid.Empty : playerInfo.Player.DefaultWalletId,
                        CreatedOn = sysDateTimeOffset,
                        ClosedOn = sysDateTimeOffset,
                        UpdatedOn = sysDateTimeOffset,
                        BrandId = (playerInfo.Player == null) ? Guid.Empty : playerInfo.Player.BrandId,
                        ExternalCreatedOn = recordBet.CreateTime,
                        ExternalClosedOn = recordBet.UpdateTime,
                        BetAmount = recordBet.Cost,
                        ValidBetAmount = recordBet.State == 1 ? 0 : recordBet.Cost,
                        PlayerWinLoss =
                           recordBet.State == 1
                               ? 0
                               :
                               _service.GetRoundStatus(recordBet.State) == RoundStatus.Closed
                                   ?
                                   recordBet.PlayerPrize - recordBet.Cost
                                   : null,
                        Commission = 0,
                        Turnover = recordBet.State == 1 ? 0 : recordBet.Cost,
                        SettleAmount = recordBet.PlayerPrize,
                        TieBetAdjustment = null,
                        ClientStatus = _service.GetClientRound(recordBet.State),
                        AdditionalData = JsonSerializer.Serialize(recordBet),
                        Success = success
                    });
                    ;
                }
            }
        }
    }
}
