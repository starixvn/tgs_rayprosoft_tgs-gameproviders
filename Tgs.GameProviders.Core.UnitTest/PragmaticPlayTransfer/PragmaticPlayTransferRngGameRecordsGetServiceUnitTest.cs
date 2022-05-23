using System;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.PragmaticPlayTransfer;
using Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;
using Tgs.GameProviders.Core.PragmaticPlayTransfer.Models;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.UnitTest.PragmaticPlayTransfer;

internal class PragmaticPlayTransferRngGameRecordsGetServiceUnitTest
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
            ExternalGameId = "vs25bkofkngdm",
            ExternalRoundId = "2806980613",
            ParentExternalRoundId = null,
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
                     Status = RoundStatus.Closed,
                     ClientStatus = RoundClientStatus.Loss,
                     ExternalRoundId = "2806980613",
                     ExternalGameId = "vs25bkofkngdm",
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

        var query = new PragmaticPlayTransferRngGameRecordsQuery(
            DateTimeOffset.Parse("2022-05-13T08:00:00+00:00"));

        _ = fakeService.GetSettingConfigurationAsync().Returns(configuration);
        _ = fakeService.GetGameProviderIdAsync().Returns(gameProviderId);

        _ = fakeApiClient
            .GetGameRoundRecordsAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(configuration.SecureLogin),
                Arg.Is(configuration.SecretKey),
                Arg.Is("RNG"),
                Arg.Is(query.StartTime))
            .Returns((success, gameRecordInfoResponse));

        var sut = new PragmaticPlayTransferRngGameRecordsGetService(fakeService, fakeApiClient);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(expected.Success, actual.Success);
        Assert.AreEqual(expected.NextTime, actual.NextTime);
        CollectionAssert.AreEqual(expected.Datas, actual.Datas);
    }

    [Test]
    public async Task Normal_Process_With_FreeSpin()
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

        var normalRound = new PragmaticPlayTransferGameRecordData
        {
            PlayerCode = "4ucmqplxb029",
            ExternalGameId = "vs25bkofkngdm",
            ExternalRoundId = "2806980613",
            ParentExternalRoundId = null,
            StartDate = "2022-05-13 08:04:41",
            EndDate = "2022-05-13 08:04:41",
            Status = "C",
            PlayType = "R",
            CurrencyCode = "CNY",
            BetAmount = 2.50m,
            WinAmount = 0.00m,
            Jackpot = 0.00m
        };

        var freeSpinRound = new PragmaticPlayTransferGameRecordData
        {
            PlayerCode = "4ucmqplxb029",
            ExternalGameId = "vs25bkofkngdm",
            ExternalRoundId = "2806980681",
            ParentExternalRoundId = "2806980613",
            StartDate = "2022-05-13 08:04:41",
            EndDate = "2022-05-13 08:04:41",
            Status = "C",
            PlayType = "F",
            CurrencyCode = "CNY",
            BetAmount = 0.00m,
            WinAmount = 0.00m,
            Jackpot = 0.00m
        };

        var gameRecordInfoResponse = new PragmaticPlayTransferGameRecordInfoResponse(
            DateTimeOffset.FromUnixTimeMilliseconds(timepoint),
            new PragmaticPlayTransferGameRecordData[] { normalRound, freeSpinRound });

        var gameProviderCode = GameProviderType.PragmaticPlayTransfer;
        var gameProviderId = Guid.Parse("e4ecfa98-b373-4123-87e0-cb9e883d0691");

        var additionalData = JsonSerializer.Serialize(normalRound);

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
                     Status = RoundStatus.Closed,
                     ClientStatus = RoundClientStatus.Loss,
                     ExternalRoundId = "2806980613",
                     ExternalGameId = "vs25bkofkngdm",
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

        var query = new PragmaticPlayTransferRngGameRecordsQuery(
            DateTimeOffset.Parse("2022-05-13T08:00:00+00:00"));

        _ = fakeService.GetSettingConfigurationAsync().Returns(configuration);
        _ = fakeService.GetGameProviderIdAsync().Returns(gameProviderId);

        _ = fakeApiClient
            .GetGameRoundRecordsAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(configuration.SecureLogin),
                Arg.Is(configuration.SecretKey),
                Arg.Is("RNG"),
                Arg.Is(query.StartTime))
            .Returns((success, gameRecordInfoResponse));

        var sut = new PragmaticPlayTransferRngGameRecordsGetService(fakeService, fakeApiClient);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(expected.Success, actual.Success);
        Assert.AreEqual(expected.NextTime, actual.NextTime);
        Assert.AreEqual(1, actual.Datas.Count());
        CollectionAssert.AreEqual(expected.Datas, actual.Datas);
    }
}