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

internal class PragmaticPlayTransferGameRecordListGetServiceUnitTest
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

        var timepoint = 1652429638085L;

        var success = true;
        var csvData =
            @$"timepoint={timepoint}
        playerID,extPlayerID,gameID,playSessionID,parentSessionID,startDate,endDate,status,type,bet,win,currency,jackpot
        2953411,4ucmqplxb029,vs25bkofkngdm,2806980613,null,2022-05-13 08:04:41,2022-05-13 08:04:41,C,R,2.50,0.00,CNY,0.00";

        var gameProviderCode = GameProviderType.PragmaticPlayTransfer;
        var gameProviderId = Guid.Parse("e4ecfa98-b373-4123-87e0-cb9e883d0691");

        var additionalData = new
        {
            ExternalRoundId = "2806980613",
            ParentExternalRoundId = "null",
            PlayType = "Round"
        };

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
                     AdditionalData = JsonSerializer.Serialize(additionalData)
                 }
             }
        };

        var query = new PragmaticPlayTransferGameRecordListQuery(
            PragmaticPlayTransferGameDataType.RNG,
            DateTimeOffset.Parse("2022-05-13T08:00:00+00:00"));

        _ = fakeService.GetSettingConfigurationAsync().Returns(configuration);
        _ = fakeService.GetGameProviderIdAsync().Returns(gameProviderId);

        _ = fakeApiClient
            .GetGameRoundRecordsAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(configuration.SecureLogin),
                Arg.Is(configuration.SecretKey),
                Arg.Is(query.DataType.ToString()),
                Arg.Is(query.StartTime))
            .Returns((success, csvData));

        var sut = new PragmaticPlayTransferGameRecordListGetService(fakeService, fakeApiClient);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(expected.Success, actual.Success);
        Assert.AreEqual(expected.NextTime, actual.NextTime);
        CollectionAssert.AreEqual(expected.Datas, actual.Datas);
    }
}