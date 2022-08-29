using System;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.PragmaticPlayTransfer;
using Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;
using Tgs.GameProviders.Core.PragmaticPlayTransfer.Models;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameRounds.Common;

namespace Tgs.GameProviders.Core.UnitTest.PragmaticPlayTransfer;

internal class PragmaticPlayTransferFishingGameRecordsGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeService = Substitute.For<IPragmaticPlayTransferService>();
        var fakeApiClient = Substitute.For<IPragmaticPlayTransferApiClient>();

        var configuration = new PragmaticPlayTransferConfiguration
        {
            ServiceEndpointUrl = new("http://game-pp"),
            ApiUrl = new("https://api.prerelease-env.biz"),
            SecureLogin = "tgs_tgstfw",
            SecretKey = "testKey",
            ProviderId = "PragmaticPlay"
        };

        var success = true;
        var timepoint = 1652429638085L;

        var data = new PragmaticPlayTransferGameRecordData
        {
            PlayerCode = "4ucmqplxb029",
            ExternalGameId = "pp5fish",
            ExternalRoundId = "2806980613",
            ParentExternalRoundId = string.Empty,
            StartDate = "2022-05-13 08:04:41",
            EndDate = "2022-05-13 08:04:41",
            Status = "C",
            PlayType = "R",
            CurrencyCode = "CNY",
            BetAmount = 2.50m,
            WinAmount = 0.00m,
            Jackpot = 0.00m
        };

        var gameRecordInfoResponse = new PragmaticPlayTransferGameRecordInfoResponse(
            DateTimeOffset.FromUnixTimeMilliseconds(timepoint),
            new PragmaticPlayTransferGameRecordData[] { data });

        var gameProviderCode = GameProviderType.PragmaticPlayTransfer;
        var gameProviderId = Guid.Parse("e4ecfa98-b373-4123-87e0-cb9e883d0691");

        var additionalData = JsonSerializer.Serialize(data);

        var expected = new PragmaticPlayTransferGameRecordResult
        {
            Success = success,
            NextTime = DateTimeOffset.FromUnixTimeMilliseconds(timepoint),
            Datas = new GameRecordListModel[]
             {
                 new GameRecordListModel
                 {
                     GameProviderCode = gameProviderCode,
                     GameProviderId = gameProviderId,
                     PlayerCode = "4ucmqplxb029",
                     Status = GameRoundStatus.Closed,
                     ClientStatus = GameRoundClientStatus.Loss,
                     ExternalRoundId = "2806980613",
                     ExternalGameId = "pp5fish",
                     ExternalCreatedOn = DateTimeOffset.Parse("2022-05-13 08:04:41", null, DateTimeStyles.AssumeUniversal),
                     ExternalClosedOn = DateTimeOffset.Parse("2022-05-13 08:04:41", null, DateTimeStyles.AssumeUniversal),
                     BetAmount = 2.50m,
                     ValidBetAmount = 2.50m,
                     PlayerWinLoss = -2.50m,
                     Commission = 0,
                     Turnover = 2.50m,
                     SettleAmount = 0.00m,
                     TieBetAdjustment = null,
                     AdditionalData = additionalData
                 }
             }
        };

        var query = new PragmaticPlayTransferFishingGameRecordsQuery(
            DateTimeOffset.Parse("2022-05-13T08:00:00+00:00"));

        _ = fakeService.GetSettingConfigurationAsync().Returns(configuration);
        _ = fakeService.GetGameProviderIdAsync().Returns(gameProviderId);

        _ = fakeApiClient
            .GetGameRoundRecordsAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(configuration.SecureLogin),
                Arg.Is(configuration.SecretKey),
                Arg.Is("R2"),
                Arg.Is(query.StartTime))
            .Returns((success, gameRecordInfoResponse));

        var sut = new PragmaticPlayTransferFishingGameRecordsGetService(fakeService, fakeApiClient);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(expected.Success, actual.Success);
        Assert.AreEqual(expected.NextTime, actual.NextTime);
        CollectionAssert.AreEqual(expected.Datas, actual.Datas);
    }
}
