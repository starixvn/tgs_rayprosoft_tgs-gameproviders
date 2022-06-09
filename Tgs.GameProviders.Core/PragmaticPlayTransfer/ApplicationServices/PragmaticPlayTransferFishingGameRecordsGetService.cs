using System;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.PragmaticPlayTransfer.Models;
using Tgs.GameProviders.Domains;
using Tgs.GameRounds.Common;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;

internal class PragmaticPlayTransferFishingGameRecordsGetService
    : IGetService<PragmaticPlayTransferFishingGameRecordsQuery, PragmaticPlayTransferGameRecordResult>
{
    private readonly IPragmaticPlayTransferApiClient _client;
    private readonly IPragmaticPlayTransferService _service;

    public PragmaticPlayTransferFishingGameRecordsGetService(
        IPragmaticPlayTransferService service,
        IPragmaticPlayTransferApiClient client)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<PragmaticPlayTransferGameRecordResult> GetAsync(PragmaticPlayTransferFishingGameRecordsQuery query,
        CancellationToken cancellationToken = default)
    {
        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
        if (configuration == null)
        {
            throw new Exception($"The {GameProviderType.PragmaticPlayTransfer} configuration does not exist.");
        }

        var (success, response) = await _client.GetGameRoundRecordsAsync(
            configuration.ApiUrl,
            configuration.SecureLogin,
            configuration.SecretKey,
            "R2",
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

        var gameRecords = response!.GameRecords
            .Select(recordData =>
            {
                var isRoundComplete = recordData.Status is "C";
                var playerWinLoss = recordData.WinAmount - recordData.BetAmount;

                return new GameRecordListModel
                {
                    GameProviderCode = gameProviderCode,
                    GameProviderId = gameProviderId,
                    PlayerCode = recordData.PlayerCode,
                    Status = isRoundComplete ? GameRoundStatus.Closed : GameRoundStatus.Open,
                    ClientStatus = isRoundComplete ? GetClientStatus(playerWinLoss) : GameRoundClientStatus.Open,
                    ExternalRoundId = recordData.ExternalRoundId,
                    ExternalGameId = recordData.ExternalGameId,
                    ExternalCreatedOn =
                        DateTimeOffset.Parse(recordData.StartDate, null, DateTimeStyles.AssumeUniversal),
                    ExternalClosedOn = isRoundComplete
                        ? DateTimeOffset.Parse(recordData.EndDate, null, DateTimeStyles.AssumeUniversal)
                        : null,
                    BetAmount = recordData.BetAmount,
                    ValidBetAmount = recordData.BetAmount,
                    PlayerWinLoss = isRoundComplete ? playerWinLoss : null,
                    Commission = 0,
                    Turnover = isRoundComplete ? recordData.BetAmount : null,
                    SettleAmount = isRoundComplete ? recordData.WinAmount : null,
                    TieBetAdjustment = null,
                    AdditionalData = JsonSerializer.Serialize(recordData)
                };
            }).ToArray();

        return new PragmaticPlayTransferGameRecordResult
        {
            Success = true,
            NextTime = response.NextTime,
            Datas = gameRecords
        };
    }

    private static GameRoundClientStatus GetClientStatus(decimal playerWinLoss)
        => playerWinLoss switch
        {
            > 0 => GameRoundClientStatus.Win,
            _ => GameRoundClientStatus.Loss
        };
}
