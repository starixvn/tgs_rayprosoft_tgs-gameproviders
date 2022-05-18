#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.PragmaticPlayTransfer.Models;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.QueryServer.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;

internal class PragmaticPlayTransferGameRecordListGetService :
    IGetService<PragmaticPlayTransferGameRecordListQuery, PragmaticPlayTransferGameRecordResult>
{
    private readonly IPragmaticPlayTransferService _service;
    private readonly IPragmaticPlayTransferApiClient _client;

    public PragmaticPlayTransferGameRecordListGetService(
        IPragmaticPlayTransferService service,
        IPragmaticPlayTransferApiClient client)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<PragmaticPlayTransferGameRecordResult> GetAsync(PragmaticPlayTransferGameRecordListQuery query,
        CancellationToken cancellationToken = default)
    {
        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
        if (configuration == null)
        {
            throw new Exception($"The {GameProviderType.PragmaticPlayTransfer} configuration does not exist.");
        }

        var dataType = GetDataType(query.DataType);

        var (success, response) = await _client.GetGameRoundRecordsAsync(
            configuration.ApiUrl,
            configuration.SecureLogin,
            configuration.SecretKey,
            dataType,
            query.StartTime).ConfigureAwait(false);

        if (!success)
        {
            return new PragmaticPlayTransferGameRecordResult
            {
                Success = false
            };
        }

        var gameProviderCode = GameProviderType.PragmaticPlayTransfer;
        var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);

        var gameRecords = new List<GameRecordListModel>();

        foreach (var recordData in response!.GameRecords)
        {
            var isRoundComplete = recordData.Status is "C";
            var playerWinLoss = recordData.WinAmount - recordData.BetAmount;

            var playType = recordData.PlayType == "F" ? "FreeSpin" : "Round";

            var record = new GameRecordListModel
            {
                GameProviderCode = gameProviderCode,
                GameProviderId = gameProviderId,
                PlayerCode = recordData.PlayerCode,
                Status = isRoundComplete ? RoundStatus.Closed : RoundStatus.Open,
                ClientStatus = isRoundComplete ? GetClientStatus(playerWinLoss) : RoundClientStatus.Open,
                ExternalRoundId = recordData.ExternalRoundId,
                ExternalGameId = recordData.ExternalGameId,
                ExternalCreatedOn = DateTimeOffset.Parse(recordData.StartDate, null, DateTimeStyles.AssumeUniversal),
                ExternalClosedOn = isRoundComplete ?
                    DateTimeOffset.Parse(recordData.EndDate, null, DateTimeStyles.AssumeUniversal)
                    : null,
                BetAmount = recordData.BetAmount,
                ValidBetAmount = recordData.BetAmount,
                PlayerWinLoss = isRoundComplete ? playerWinLoss : null,
                Commission = 0,
                Turnover = isRoundComplete ? recordData.BetAmount : null,
                SettleAmount = isRoundComplete ? recordData.WinAmount : null,
                TieBetAdjustment = null,
                AdditionalData = JsonSerializer.Serialize(new
                {
                    recordData.ExternalRoundId,
                    recordData.ParentExternalRoundId,
                    PlayType = playType
                })
            };

            gameRecords.Add(record);

            if (recordData.Jackpot <= 0)
                continue;

            var jackpotRecord = new GameRecordListModel
            {
                GameProviderCode = gameProviderCode,
                GameProviderId = gameProviderId,
                PlayerCode = recordData.PlayerCode,
                Status = isRoundComplete ? RoundStatus.Closed : RoundStatus.Open,
                ClientStatus = isRoundComplete ? RoundClientStatus.Open : RoundClientStatus.Win,
                ExternalRoundId = $"{recordData.ExternalRoundId}_jackpot",
                ExternalGameId = recordData.ExternalGameId,
                ExternalCreatedOn = DateTimeOffset.Parse(recordData.StartDate, null, DateTimeStyles.AssumeUniversal),
                ExternalClosedOn = isRoundComplete
                    ? DateTimeOffset.Parse(recordData.EndDate, null, DateTimeStyles.AssumeUniversal)
                    : null,
                BetAmount = 0,
                ValidBetAmount = 0,
                PlayerWinLoss = isRoundComplete ? recordData.Jackpot : null,
                Commission = 0,
                Turnover = 0,
                SettleAmount = isRoundComplete ? recordData.Jackpot : null,
                TieBetAdjustment = null,
                AdditionalData = JsonSerializer.Serialize(new
                {
                    recordData.ExternalRoundId,
                    recordData.ParentExternalRoundId,
                    PlayType = playType
                })
            };

            gameRecords.Add(jackpotRecord);
        }

        return new PragmaticPlayTransferGameRecordResult
        {
            Success = true,
            NextTime = response.NextTime,
            Datas = gameRecords
        };
    }

    private static string GetDataType(PragmaticPlayTransferGameDataType dataType)
        => dataType switch
        {
            PragmaticPlayTransferGameDataType.RNG => "RNG",
            PragmaticPlayTransferGameDataType.Fishing => "R2",
            PragmaticPlayTransferGameDataType.LiveDealer => "LC",
            PragmaticPlayTransferGameDataType.VirtualSports => "VSB",
            _ => "RNG"
        };

    private static RoundClientStatus GetClientStatus(decimal playerWinLoss)
        => playerWinLoss switch
        {
            > 0 => RoundClientStatus.Win,
            _ => RoundClientStatus.Loss
        };
}