using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.ApiClients.KY.Models;
using TGS.GameProviders.ApiClients.KY.Models.Commons;
using TGS.GameProviders.Core;
using TGS.GameProviders.Domains.Extensions;
using TGS.GameProviders.GameRecord.Core.Processors;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Domains;
using TGS.GameProviders.Repositories.Domains.Models.Game;
using TGS.GameProviders.Repositories.Domains.Models.Player;
using TGS.ServiceUtility;

namespace TGS.GameProviders.GameRecord.Core.ApplicationServices.Ky
{
    public class KYGameRecordHandler : IGetService<KYGameRecordHandlerQuery, KYGameRecordHandlerResult>
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IKYApiClient _client;
        private readonly IGameRepository _gameRepository;
        private readonly IGameRecordProcessor _gameRoundsProcessor;
        private readonly IPlayerRepository _playerRepository;
        private readonly IKYService _service;

        public KYGameRecordHandler(
            IKYApiClient client,
            IKYService service,
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

        public async ValueTask<KYGameRecordHandlerResult> GetAsync(KYGameRecordHandlerQuery query, CancellationToken cancellationToken = default)
        {
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
            var brandMerchant = await _service.GetBrandMerchantAsync(query.BrandCode);
            var agentId = brandMerchant.AgentMappers.Where(x => x.CurrencyCode.IgnoreCaseEquals(query.CurrencyCode) && x.Status).FirstOrDefault()?.AgentId;
            if (string.IsNullOrEmpty(agentId))
            {
                return new KYGameRecordHandlerResult()
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

            var response = await _client.GetGameRecordAsync(brandMerchant.ApiRecordUrl, brandMerchant.Deskey, brandMerchant.Md5key, agentId, dateTimeOffset, data);

            var players = new Dictionary<string, (GetPlayer Player, Guid? GameProviderConfigurationId)>();
            var games = new Dictionary<int, GetGame>();
            var count = 0;
            var successCount = 0;
            var failCount = 0;
            var lastGameRecordTime = default(DateTimeOffset?);
            if (response.Data.Code == 0)
            {
                var sysDateTimeOffset = DateTimeOffset.UtcNow;

                for (int i = 0; i < response.Data.Count; i++)
                {
                    count++;

                    var gameRecord = new KYGameRecord()
                    {
                        GameID = response.Data.List.GameID[i],
                        Account = response.Data.List.Accounts[i],
                        ServerID = response.Data.List.ServerID[i],
                        KindID = response.Data.List.KindID[i],
                        TableID = response.Data.List.TableID[i],
                        ChairID = response.Data.List.ChairID[i],
                        UserCount = response.Data.List.UserCount[i],
                        CellScore = Convert.ToDecimal(response.Data.List.CellScore[i]),
                        AllBet = Convert.ToDecimal(response.Data.List.AllBet[i]),
                        Profit = Convert.ToDecimal(response.Data.List.Profit[i]),
                        Revenue = Convert.ToDecimal(response.Data.List.Revenue[i]),
                        GameStartTime = new DateTimeOffset(DateTime.Parse(response.Data.List.GameStartTime[i]), TimeSpan.FromHours(+8)),
                        GameEndTime = new DateTimeOffset(DateTime.Parse(response.Data.List.GameEndTime[i]), TimeSpan.FromHours(+8)),
                        CardValue = response.Data.List.CardValue[i],
                        ChannelID = Convert.ToInt32(response.Data.List.ChannelID[i]),
                        LineCode = response.Data.List.LineCode[i]
                    };

                    var success = true;
                    var playerCode = gameRecord.Account.Split('_')[1];

                    if (!players.TryGetValue(playerCode, out (GetPlayer Player, Guid? GameProviderConfigurationId) playerInfo))
                    {
                        var player = await _playerRepository.GetPlayerAsync(playerCode).ConfigureAwait(false);
                        var brandGameProviderConfigurationId = default(Guid?);
                        if (player != null)
                        {
                            brandGameProviderConfigurationId = await _brandRepository.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
                        }
                        playerInfo = new(player, brandGameProviderConfigurationId);
                        players.Add(playerCode, playerInfo);
                    }

                    if (!games.TryGetValue(gameRecord.KindID, out GetGame game))
                    {
                        game = await _gameRepository.GetGameAsync(gameProviderId, gameRecord.KindID.ToString()).ConfigureAwait(false);
                        games.Add(gameRecord.KindID, game);
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
                        ExternalRoundId = gameRecord.GameID,
                        GameProviderId = gameProviderId,
                        GameProviderCode = _service.GetGameProviderCode(),
                        PlayerCode = (playerInfo.Player == null) ? string.Empty : playerInfo.Player.PlayerCode,
                        GameId = (game == null) ? -1 : game.GameId,
                        ExternalId = gameRecord.KindID.ToString(),
                        CurrencyCode = (playerInfo.Player == null) ? string.Empty : playerInfo.Player.DefaultWalletCurrencyCode,
                        Status = RoundStatus.Closed,
                        GameProviderConfigurationId = playerInfo.GameProviderConfigurationId.GetValueOrDefault(),
                        WalletId = (playerInfo.Player == null) ? Guid.Empty : playerInfo.Player.DefaultWalletId,
                        CreatedOn = sysDateTimeOffset,
                        ClosedOn = sysDateTimeOffset,
                        UpdatedOn = sysDateTimeOffset,
                        BrandId = (playerInfo.Player == null) ? Guid.Empty : playerInfo.Player.BrandId,
                        ExternalCreatedOn = gameRecord.GameStartTime,
                        ExternalClosedOn = gameRecord.GameEndTime,
                        BetAmount = gameRecord.AllBet,
                        ValidBetAmount = gameRecord.AllBet,
                        PlayerWinLoss = gameRecord.Profit,
                        Commission = gameRecord.Revenue,
                        Turnover = gameRecord.CellScore,
                        SettleAmount = gameRecord.AllBet + gameRecord.Profit,
                        TieBetAdjustment = null,
                        ClientStatus = _service.GetClientRound(gameRecord.Profit),
                        AdditionalData = JsonSerializer.Serialize(gameRecord),
                        Success = success
                    });

                    lastGameRecordTime = new DateTimeOffset(DateTime.Parse(response.Data.List.GameEndTime[i]), TimeSpan.FromHours(+8));
                }
            }

            var responses = new KYGameRecordHandlerResult()
            {
                Success = (response.Data.Code is 0 or 16),
                LastGameRecorTime = lastGameRecordTime,
                Count = (response.Data.Code is 0 or 16) ? count : 0,
                SuccessCount = successCount,
                FailCount = failCount
            };
            return responses;
        }
    }
}
