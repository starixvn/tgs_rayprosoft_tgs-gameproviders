using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TGS.GameProviders.ApiClients.WGHorse;
using TGS.GameProviders.ApiClients.WGHorse.Models;
using TGS.GameProviders.Core;
using TGS.GameProviders.Domains.Extensions;
using TGS.GameProviders.GameRecord.Core.Processors;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Domains;
using TGS.GameProviders.Repositories.Domains.Models.Player;
using TGS.ServiceUtility;

namespace TGS.GameProviders.GameRecord.Core.ApplicationServices.WGHorse
{
    public class WGHorseGameRecordHandler : IGetService<WGHorseGameRecordHandlerQuery, WGHorseGameRecordHandlerResult>
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IWGHorseApiClient _client;
        private readonly IGameRecordProcessor _gameRoundsProcessor;
        private readonly IPlayerRepository _playerRepository;
        private readonly IWGHorseService _service;

        public WGHorseGameRecordHandler(
            IWGHorseApiClient client,
            IWGHorseService service,
            IBrandRepository brandRepository,
            IPlayerRepository playerRepository,
            IGameRecordProcessor gameRoundsProcessor
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _gameRoundsProcessor = gameRoundsProcessor ?? throw new ArgumentNullException(nameof(gameRoundsProcessor));
        }

        public async ValueTask<WGHorseGameRecordHandlerResult> GetAsync(WGHorseGameRecordHandlerQuery query, CancellationToken cancellationToken = default)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
            var players = new Dictionary<string, (GetPlayer Player, Guid? GameProviderConfigurationId)>();
            var game = await _service.GetGameByExternalGameId(gameProviderId, "1").ConfigureAwait(false);
            var count = 0;
            var successCount = 0;
            var failCount = 0;
            var lastGameRecordTime = default(DateTimeOffset?);
            var page = 1;
            var rows = 1000;
            var success = true;
            while (true)
            {
                var data = new WGHorseGameRecordRequest
                {
                    StartTime = query.StartTime,
                    EndTime = query.EndTime,
                    PageIndex = page,
                    PageSize = rows
                };

                var response = await _client.GetGameRecordAsync(configuration, data).ConfigureAwait(false);

                if (response.ReturnCode != 1)
                {
                    if (response.ReturnCode != 503)
                    {
                        break;
                    }
                }

                if (page.ToString().IgnoreCaseNotEquals(response.Page))
                {
                    break;
                }

                if (response.ArrayInvoice == null || response.ArrayInvoice.Length == 0)
                {
                    break;
                }

                page++;

                var sysDateTimeOffset = DateTimeOffset.UtcNow;
                var records = response.ArrayInvoice;

                foreach (var record in records)
                {
                    count++;
                    var gameRecord = record;

                    if (!players.TryGetValue(gameRecord.MemberCode, out (GetPlayer Player, Guid? GameProviderConfigurationId) playerInfo))
                    {
                        var player = await _playerRepository.GetPlayerAsync(gameRecord.MemberCode).ConfigureAwait(false);
                        var brandGameProviderConfigurationId = default(Guid?);
                        if (player != null)
                        {
                            brandGameProviderConfigurationId = await _brandRepository.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
                        }
                        playerInfo = new(player, brandGameProviderConfigurationId);
                        players.Add(gameRecord.MemberCode, playerInfo);
                    }

                    if (playerInfo.Player == null)
                    {
                        success = false;
                        failCount++;
                    }
                    else
                    {
                        successCount++;
                    }

                    var createdOn = new DateTimeOffset(DateTime.Parse(gameRecord.RecordDate), TimeSpan.FromHours(+8));
                    var externalClosedOn = new DateTimeOffset(DateTime.Parse(gameRecord.ShowDivideDate), TimeSpan.FromHours(+8));
                    await _gameRoundsProcessor.ExecuteAsync(new GameRoundsData()
                    {
                        ExternalRoundId = gameRecord.BatchNo,
                        GameProviderId = gameProviderId,
                        GameProviderCode = _service.GetGameProviderCode(),
                        PlayerCode = (playerInfo.Player == null) ? string.Empty : playerInfo.Player.PlayerCode,
                        GameId = (game == null) ? -1 : game.GameId,
                        ExternalId = gameRecord.BatchNo.ToString(),
                        CurrencyCode = (playerInfo.Player == null) ? string.Empty : playerInfo.Player.DefaultWalletCurrencyCode,
                        Status = RoundStatus.Closed,
                        GameProviderConfigurationId = playerInfo.GameProviderConfigurationId.GetValueOrDefault(),
                        WalletId = (playerInfo.Player == null) ? Guid.Empty : playerInfo.Player.DefaultWalletId,
                        CreatedOn = sysDateTimeOffset,
                        ClosedOn = sysDateTimeOffset,
                        UpdatedOn = sysDateTimeOffset,
                        BrandId = (playerInfo.Player == null) ? Guid.Empty : playerInfo.Player.BrandId,
                        ExternalCreatedOn = createdOn,
                        ExternalClosedOn = externalClosedOn,
                        BetAmount = decimal.Parse(gameRecord.ShowAmount),
                        ValidBetAmount = decimal.Parse(gameRecord.ShowAmount),
                        PlayerWinLoss = decimal.Parse(gameRecord.ShowWin),
                        Commission = 0,
                        Turnover = decimal.Parse(gameRecord.ShowAmount),
                        SettleAmount = decimal.Parse(gameRecord.ShowAmount) + decimal.Parse(gameRecord.ShowWin),
                        TieBetAdjustment = null,
                        ClientStatus = _service.GetClientRound(decimal.Parse(gameRecord.ShowWin)),
                        AdditionalData = JsonSerializer.Serialize(gameRecord),
                        Success = success
                    });
                    lastGameRecordTime = externalClosedOn;
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
