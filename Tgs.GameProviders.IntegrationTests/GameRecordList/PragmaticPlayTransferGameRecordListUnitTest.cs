using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.PragmaticPlayTransfer;
using Tgs.GameProviders.Core.PragmaticPlayTransfer.Models;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;
using Tgs.GameProviders.ViewModels;

namespace Tgs.GameProviders.IntegrationTests.GameRecordList;

internal class PragmaticPlayTransferGameRecordListUnitTest
{
    [Test]
    public async Task Test_PragmaticPlayTransfer_GameRecordListAsync()
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
        var gameRecordInfoResponse = new PragmaticPlayTransferGameRecordInfoResponse(
            DateTimeOffset.FromUnixTimeMilliseconds(timepoint),
            new PragmaticPlayTransferGameRecordData[]
            {
                new PragmaticPlayTransferGameRecordData
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
                }
            });

        var gameProviderCode = GameProviderType.PragmaticPlayTransfer;
        var gameProviderId = Guid.Parse("e4ecfa98-b373-4123-87e0-cb9e883d0691");

        var success = true;

        var result = new PragmaticPlayTransferGameRecordResult
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
                    AdditionalData = null
                }
            }
        };

        var web = new TestGameProvidersApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                _ = services.AddSingleton(fakeApiClient);
                _ = services.AddSingleton(fakeService);
                _ = services.AddFakeNatsMessageQueue();
            });
        });

        var query = new PragmaticPlayTransferGameRecordListRequest
        {
            DataType = Core.PragmaticPlayTransfer.PragmaticPlayTransferGameDataType.RNG,
            StartTime = DateTimeOffset.Parse("2021-04-20T00:00:00+00:00")
        };

        _ = fakeService.GetSettingConfigurationAsync().Returns(configuration);

        _ = fakeService.GetGameProviderIdAsync().Returns(gameProviderId);

        _ = fakeApiClient
            .GetGameRoundRecordsAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(configuration.SecureLogin),
                Arg.Is(configuration.SecretKey),
                Arg.Any<string>(),
                Arg.Is(query.StartTime))
            .Returns((success, gameRecordInfoResponse));

        var fakeLoggingHandler = new LoggingHandler(web.Services.GetRequiredService<ILogger<LoggingHandler>>());

        var httpClient = web.CreateDefaultClient(new DelegatingHandler[] { fakeLoggingHandler });

        var sut = new GameRecordListClient(httpClient);

        var actual = await sut
            .PragmaticPlayTransferGameRecordListAsync(
                Client.PragmaticPlayTransferGameDataType.RNG,
                query.StartTime)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(true, actual!.Success);
        Assert.AreEqual(gameProviderCode, actual.Datas!.ElementAt(0).GameProviderCode);
        Assert.AreEqual(gameProviderId, actual.Datas!.ElementAt(0).GameProviderId);
        Assert.AreEqual(result.Datas.ElementAt(0).ExternalRoundId, actual.Datas!.ElementAt(0).ExternalRoundId);
    }
}