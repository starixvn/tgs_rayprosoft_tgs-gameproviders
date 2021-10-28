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
        private readonly IBigGamingApiClient _client;
        private readonly IGameRepository _gameRepository;
        private readonly IGameRecordProcessor _gameRoundsProcessor;
        private readonly IPlayerRepository _playerRepository;
        private readonly IBigGamingService _service;

        public BigGamingFishingGameRecordHandler(
            IBigGamingApiClient client,
            IBigGamingService service,
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

        public async ValueTask<BigGamingFishingGameRecordHandlerResult> GetAsync(BigGamingFishingGameRecordHandlerQuery query, CancellationToken cancellationToken = default)
        {
            var brandMerchant = await _service.GetBrandMerchantAsync(query.CurrencyCode).ConfigureAwait(false); ;
            int pageIndex = 1;
            int pageSize = 500;
            var gameProviderId = await _service.GetGameProviderIdAsync();
            var players = new Dictionary<string, (GetPlayer Player, Guid? BrandGameProviderConfigurationIds)>();
            var games = new Dictionary<int, GetGame>();
            var count = 0;
            var successCount = 0;
            var failCount = 0;
            var lastGameRecordTime = default(DateTimeOffset?);
            var sysDateTimeOffset = DateTime.UtcNow;
            var request = new BigGamingGameRecordFishingRequest()
            {
                Random = GuidExtensions.NewSequentialGuid().ToString(),
                Sn = brandMerchant.SecretCode,
                StartTime = query.StartTime.ToOffset(TimeSpan.FromHours(-4)).ToString("yyyy-MM-dd HH:mm:ss"),
                EndTime = query.EndTime.ToOffset(TimeSpan.FromHours(-4)).ToString("yyyy-MM-dd HH:mm:ss"),
                PageIndex = pageIndex.ToString(),
                PageSize = pageSize
            };

            while (true)
            {
                var response = await _client.GetGameRecordAsync(brandMerchant.ApiUrl, brandMerchant.SecretCode, brandMerchant.SecretKey, request).ConfigureAwait(false);
                if (response.Error != null)
                {
                    return new()
                    {
                        Success = false
                    };
                }

                var gameRecords = JsonSerializer.Deserialize<BigGamingResultGameRecordResponse<BigGamingGameRecordFishingResponse>>(JsonSerializer.Serialize(response.Result));

                if (gameRecords.Items == null || gameRecords.Items.Length == 0)
                {
                    break;
                }
                pageIndex++;
                request.PageIndex = pageIndex.ToString();
                var gameRecordItems = gameRecords.Items;
                for (int i = 0; i < gameRecordItems.Length; i++)
                {
                    count++;

                    var success = true;
                    var gameRecord = gameRecordItems[i];

                    if (!players.TryGetValue(gameRecord.LoginId, out (GetPlayer Player, Guid? GameProviderConfigurationId) playerInfo))
                    {
                        var player = await _playerRepository.GetPlayerAsync(gameRecord.LoginId).ConfigureAwait(false);
                        var brandGameProviderConfigurationId = default(Guid?);
                        if (player != null)
                        {
                            brandGameProviderConfigurationId = await _brandRepository.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
                        }
                        playerInfo = new(player, brandGameProviderConfigurationId);
                        players.Add(gameRecord.LoginId, playerInfo);
                    }

                    if (!games.TryGetValue(gameRecord.GameType, out GetGame game))
                    {
                        game = await _gameRepository.GetGameAsync(gameProviderId, gameRecord.GameType.ToString()).ConfigureAwait(false);
                        games.Add(gameRecord.GameType, game);
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

                    var orderTime = new DateTimeOffset(DateTime.Parse(gameRecord.OrderTime), TimeSpan.FromHours(-4));
                    await _gameRoundsProcessor.ExecuteAsync(new GameRoundsData()
                    {
                        ExternalRoundId = gameRecord.ExternalBetId.ToString(),
                        GameProviderId = gameProviderId,
                        GameProviderCode = _service.GetGameProviderCode(),
                        PlayerCode = (playerInfo.Player == null) ? string.Empty : playerInfo.Player.PlayerCode,
                        GameId = game != null ? game.GameId : -1,
                        ExternalId = gameRecord.ExternalBetId.ToString(),
                        CurrencyCode = (playerInfo.Player == null) ? string.Empty : playerInfo.Player.DefaultWalletCurrencyCode,
                        Status = RoundStatus.Closed,
                        GameProviderConfigurationId = playerInfo.GameProviderConfigurationId.GetValueOrDefault(),
                        WalletId = (playerInfo.Player == null) ? Guid.Empty : playerInfo.Player.DefaultWalletId,
                        CreatedOn = sysDateTimeOffset,
                        ClosedOn = sysDateTimeOffset,
                        UpdatedOn = sysDateTimeOffset,
                        BrandId = (playerInfo.Player == null) ? Guid.Empty : playerInfo.Player.BrandId,
                        ExternalCreatedOn = orderTime,
                        ExternalClosedOn = orderTime,
                        BetAmount = gameRecord.BetAmount,
                        ValidBetAmount = gameRecord.ValidAmount,
                        PlayerWinLoss = gameRecord.Payout,
                        Commission = gameRecord.Extend,
                        Turnover = gameRecord.ValidAmount,
                        SettleAmount = gameRecord.ValidAmount + gameRecord.Payout,
                        TieBetAdjustment = null,
                        ClientStatus = _service.GetClientRound(gameRecord.Payout),
                        AdditionalData = JsonSerializer.Serialize(gameRecord),
                        Success = success
                    });
                    lastGameRecordTime = orderTime;
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
