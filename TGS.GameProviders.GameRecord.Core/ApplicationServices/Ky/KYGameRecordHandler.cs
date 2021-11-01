using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.GameProviders.ApiClients.KY.Models;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.KY.ApplicationServices;
using TGS.GameProviders.Domains.Extensions;
using TGS.GameProviders.GameRecord.Core.Processors;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Domains.Models.Game;
using TGS.GameProviders.Repositories.Domains.Models.Player;

namespace TGS.GameProviders.GameRecord.Core.ApplicationServices.Ky
{
    public class KYGameRecordHandler : IGetService<KYGameRecordHandlerQuery, KYGameRecordHandlerResult>
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IGameRecordProcessor _gameRoundsProcessor;
        private readonly IGetService<KYGameRecordListQuery, GameRecordListResult> _getService;
        private readonly IPlayerRepository _playerRepository;
        private readonly IKYService _service;

        public KYGameRecordHandler(
            IKYService service,
            IGameRepository gameRepository,
            IBrandRepository brandRepository,
            IPlayerRepository playerRepository,
            IGameRecordProcessor gameRoundsProcessor,
            IGetService<KYGameRecordListQuery, GameRecordListResult> getService
            )
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _gameRoundsProcessor = gameRoundsProcessor ?? throw new ArgumentNullException(nameof(gameRoundsProcessor));
            _getService = getService ?? throw new ArgumentNullException(nameof(getService));
        }

        public async ValueTask<KYGameRecordHandlerResult> GetAsync(KYGameRecordHandlerQuery query, CancellationToken cancellationToken = default)
        {
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
            var brandMerchant = await _service.GetBrandMerchantAsync(query.BrandCode);
            var agentId = brandMerchant.AgentMappers.Where(x => x.CurrencyCode.IgnoreCaseEquals(query.CurrencyCode) && x.Status).FirstOrDefault()?.AgentId;
            if (string.IsNullOrEmpty(agentId))
            {
                return new()
                {
                    Success = false
                };
            }

            var dateTimeOffset = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));

            var data = new KYGameRecordRequest()
            {
                StartTime = query.StartTime.ToUnixTimeMilliseconds(),
                EndTime = query.EndTime.ToUnixTimeMilliseconds()
            };

            var response = await _getService.GetAsync(new KYGameRecordListQuery()
            {
                BrandCode = query.BrandCode,
                CurrencyCode = query.CurrencyCode,
                StartTime = query.StartTime,
                EndTime = query.EndTime
            });

            if (!response.Success)
            {
                return new() { Success = false };
            }

            if (response.Success && response.Datas == null)
            {
                return new() { Success = true };
            }
            var players = new Dictionary<string, (GetPlayer Player, Guid? GameProviderConfigurationId)>();
            var games = new Dictionary<string, GetGame>();
            var successCount = 0;
            var failCount = 0;
            var lastGameRecordTime = default(DateTimeOffset?);

            var sysDateTimeOffset = DateTimeOffset.UtcNow;

            foreach (var gameRecord in response.Datas)
            {
                var success = true;

                if (!players.TryGetValue(gameRecord.PlayerCode, out (GetPlayer Player, Guid? GameProviderConfigurationId) playerInfo))
                {
                    var player = await _playerRepository.GetPlayerAsync(gameRecord.PlayerCode).ConfigureAwait(false);
                    var brandGameProviderConfigurationId = default(Guid?);
                    if (player != null)
                    {
                        brandGameProviderConfigurationId = await _brandRepository.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
                    }
                    playerInfo = new(player, brandGameProviderConfigurationId);
                    players.Add(gameRecord.PlayerCode, playerInfo);
                }

                if (!games.TryGetValue(gameRecord.ExternalGameId, out GetGame game))
                {
                    game = await _gameRepository.GetGameAsync(gameProviderId, gameRecord.ExternalGameId).ConfigureAwait(false);
                    games.Add(gameRecord.ExternalGameId, game);
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

                var result = await _gameRoundsProcessor.ExecuteAsync(new GameRoundsData()
                {
                    ExternalRoundId = gameRecord.ExternalRoundId,
                    GameProviderId = gameProviderId,
                    GameProviderCode = _service.GetGameProviderCode(),
                    PlayerCode = (playerInfo.Player == null) ? string.Empty : playerInfo.Player.PlayerCode,
                    GameId = (game == null) ? -1 : game.GameId,
                    ExternalGameId = gameRecord.ExternalGameId,
                    CurrencyCode = (playerInfo.Player == null) ? string.Empty : playerInfo.Player.DefaultWalletCurrencyCode,
                    Status = gameRecord.Status,
                    GameProviderConfigurationId = playerInfo.GameProviderConfigurationId.GetValueOrDefault(),
                    WalletId = (playerInfo.Player == null) ? Guid.Empty : playerInfo.Player.DefaultWalletId,
                    CreatedOn = sysDateTimeOffset,
                    ClosedOn = sysDateTimeOffset,
                    UpdatedOn = sysDateTimeOffset,
                    BrandId = (playerInfo.Player == null) ? Guid.Empty : playerInfo.Player.BrandId,
                    ExternalCreatedOn = gameRecord.ExternalCreatedOn,
                    ExternalClosedOn = gameRecord.ExternalClosedOn,
                    BetAmount = gameRecord.BetAmount,
                    ValidBetAmount = gameRecord.ValidBetAmount,
                    PlayerWinLoss = gameRecord.PlayerWinLoss,
                    Commission = gameRecord.Commission,
                    Turnover = gameRecord.Turnover,
                    SettleAmount = gameRecord.ValidBetAmount + gameRecord.PlayerWinLoss,
                    TieBetAdjustment = null,
                    ClientStatus = gameRecord.ClientStatus,
                    AdditionalData = gameRecord.AdditionalData,
                    Success = success
                }).ConfigureAwait(false);

                lastGameRecordTime = gameRecord.ExternalClosedOn;
            };

            return new()
            {
                Success = response.Success,
                LastGameRecorTime = lastGameRecordTime,
                Count = response.Datas.Count(),
                SuccessCount = successCount,
                FailCount = failCount
            };
        }
    }
}
