using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.QueryServer.Models;
using TGS.GameProviders.Core.Saba;
using TGS.GameProviders.Core.Saba.ApplicationServices;
using TGS.GameProviders.Core.Saba.Models;
using TGS.GameProviders.Domains.RoundDetail;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.UnitTests.Saba;

[TestFixture]
public class SabaRoundDetailGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<SabaRoundDetailGetService>.Instance;
        var fakeService = Substitute.For<ISabaService>();
        var fakeApiClient = Substitute.For<ISabaApiClient>();
        var fakeGameActionService = Substitute.For<IGameActionService>();
        var fakeRoundService = Substitute.For<IRoundService>();

        var configuration = new SabaConfiguration
        {
            ServiceEndpointUrl = new("http://game-saba"),
            ApiUrl = new("http://t829tsa.bw6688.com"),
            OperatorId = "ZhiFeng055",
            VendorId = "zzdaa12nu1",
            SecretKey = "SecretKey",
            IsTestEnvironment = false
        };

        var playerInfo = new PlayerInfo(
         Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
         "TGS",
         Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
         "TGS",
         Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
         "GPI",
         "4ucmqplxb029",
         "4ucmqplxb029",
         Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
         "CNY",
         PlayerStatus.Enabled);

        var gameActionInfo = new GameActionInfo(
            Guid.NewGuid(),
            Guid.NewGuid(),
            playerInfo.PlayerCode,
            "1639679431950180036",
            GameActionType.PlaceBet,
            20m,
            0m,
            0m,
            0m,
            0m,
            "1639679431950180036",
            "1639679431950180036",
            null,
            null,
            null,
            DateTime.UtcNow,
            null,
            false,
            null,
            "::1",
            null,
            null,
            DateTime.UtcNow);

        var historyData = new SabaBetHistoryData
        {
            VersionKey = 1000,
            Results = new SabaGameResult[]
            {
                new()
                {
                    ExternalTransactionId = 1639679431950180036,
                    PlayerCode = playerInfo.PlayerCode,
                    LeagueId = 1,
                    LeagueName = new SabaName[]
                    {
                        new()
                        {
                            Language = "en",
                            Value = "LeagueName"
                        }
                    },
                    MatchId = 1000,
                    HomeId = 10,
                    HomeTeamName = new SabaName[]
                    {
                       new()
                       {
                           Language = "en",
                           Value = "Home Team"
                       }
                    },
                    AwayId = 20,
                    AwayTeamName = new SabaName[]
                    {
                        new()
                        {
                            Language = "en",
                            Value = "Away Team"
                        }
                    },
                    GameId = "1639679431950180036",
                    TransactionTime = DateTime.UtcNow.ToString("u"),
                    TicketStatus = "win",
                    WinlostAmount = 25m
                }
            }
        };

        var roundInfo = new RoundInfo(
            gameActionInfo.RoundId,
            "1639679431950180036",
            Guid.Parse("09d8d244-206b-4030-8110-b2ea90dc9d2f"),
            playerInfo.PlayerCode,
            600001,
            playerInfo.DefaultWalletCurrencyCode,
            Tgs.GameProviders.QueryServer.Models.RoundStatus.Closed,
            Guid.NewGuid(),
            playerInfo.DefaultWalletId,
            DateTime.Now.AddMinutes(-1),
            DateTime.Now,
            DateTime.Now,
            playerInfo.BrandId,
            DateTime.Now.AddMinutes(-1),
            DateTime.Now,
            20m,
            20m,
            45m,
            null,
            20m,
            -25m,
            null,
            Tgs.GameProviders.QueryServer.Models.RoundClientStatus.Win,
            null,
            null,
            null,
            JsonSerializer.Serialize(historyData));

        var query = new SabaRoundDetailQuery
        {
            Language = "en-US",
            RoundId = roundInfo.RoundId
        };

        var gameActions = new[]
        {
            gameActionInfo
        }.ToAsyncEnumerable();

        var expected = new RoundDetailResult
        {
            Data = new RoundDetailDataBase[]
            {
                new RoundDetailDataJson
                {
                    Value = JsonSerializer.Serialize(historyData.Results)
                }
            }
        };

        _ = fakeRoundService.GetAsync(query.RoundId).Returns(ValueTask.FromResult(roundInfo));
        _ = fakeGameActionService.QueryAsync(query.RoundId).Returns(gameActions);
        _ = fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        _ = fakeApiClient
            .GetBetDetailByTransIdAsync(
                configuration.ApiUrl,
                new SabaGetRoundHistoryRequest
                {
                    ExternalTransactionId = gameActionInfo.ExternalTransactionId,
                    VendorId = configuration.VendorId
                })
            .Returns(ValueTask.FromResult(new SabaGetRoundHistoryResponse
            {
                ErrorCode = SabaGetRoundHistoryApiError.Ok,
                Data = historyData
            }));

        var sut = new SabaRoundDetailGetService(
            fakeLogger,
            fakeRoundService,
            fakeGameActionService,
            fakeService,
            fakeApiClient);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        CollectionAssert.AreEqual(expected.Data, actual.Data);
    }
}
