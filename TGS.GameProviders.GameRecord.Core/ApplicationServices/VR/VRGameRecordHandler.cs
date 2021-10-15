using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TGS.GameProviders.ApiClients.VR;
using TGS.GameProviders.ApiClients.VR.Models;
using TGS.GameProviders.Core;
using TGS.GameProviders.Domains.SettingConfiguration.VR;
using TGS.GameProviders.GameRecord.Core.Processors;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Domains;
using TGS.GameProviders.Repositories.Domains.Models.Game;
using TGS.GameProviders.Repositories.Domains.Models.Player;
using TGS.ServiceUtility;
using TGS.GameProviders.ApiClients;

namespace TGS.GameProviders.GameRecord.Core.ApplicationServices.VR
{
    public class VRGameRecordHandler : IGetService<VRGameRecordHandlerQuery, VRGameRecordHandlerResult>
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IVRApiClient _client;
        private readonly IGameRepository _gameRepository;
        private readonly IGameRecordProcessor _gameRoundsProcessor;
        private readonly IPlayerRepository _playerRepository;
        private readonly IVRService _service;

        private int _count;
        private int _failCount;
        private Dictionary<int, GetGame> _games;
        private DateTimeOffset? _lastGameRecordTime;
        private Dictionary<string, (GetPlayer Player, Guid? GameProviderConfigurationId)> _players;
        private int _successCount;

        public VRGameRecordHandler(
            IVRApiClient client,
            IVRService service,
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

        public async ValueTask<VRGameRecordHandlerResult> GetAsync(VRGameRecordHandlerQuery query, CancellationToken cancellationToken = default)
        {
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
            var brandMerchant = await _service.GetBrandMerchantAsync(query.BrandCode).ConfigureAwait(false);
            if (brandMerchant == null)
            {
                return new()
                {
                    Success = false
                };
            }

            _players = new Dictionary<string, (GetPlayer Player, Guid? GameProviderConfigurationId)>();
            _games = new Dictionary<int, GetGame>();
            _count = 0;
            _successCount = 0;
            _failCount = 0;
            _lastGameRecordTime = default;
            var unsettled = await GetGameRoundAsync(query, brandMerchant, gameProviderId, false);
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

            var settled = await GetGameRoundAsync(query, brandMerchant, gameProviderId, true);
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
        /// <param name="request">The request.</param>
        /// <param name="brandMerchant">The brand merchant.</param>
        /// <param name="gameProviderId">The game provider identifier.</param>
        /// <param name="isUpdateTime">if set to <c>true</c> [is update time].</param>
        /// <returns></returns>
        private async Task<bool> GetGameRoundAsync(VRGameRecordHandlerQuery query, VRBrandMerchant brandMerchant, Guid gameProviderId, bool isUpdateTime)
        {
            var page = 0;
            var rows = 500;
            while (true)
            {
                var data = new VRGameRecordRequest
                {
                    StartTime = query.StartTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    EndTime = query.EndTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    IsUpdateTime = isUpdateTime,
                    Page = page,
                    Rows = rows
                };

                var response = await _client.GetGameRecordAsync(brandMerchant, data);

                if (response.Records.Length == 0)
                {
                    return true;
                }

                page++;
                if (response.Records.Length > 0)
                {
                    var sysDateTimeOffset = DateTimeOffset.UtcNow;

                    foreach (var recordBet in response.Records)
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
}
