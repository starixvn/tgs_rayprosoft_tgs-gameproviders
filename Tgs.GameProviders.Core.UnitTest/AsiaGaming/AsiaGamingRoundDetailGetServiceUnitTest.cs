using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Core.AsiaGaming;
using Tgs.GameProviders.Core.AsiaGaming.ApplicationServices;
using Tgs.GameProviders.Core.AsiaGaming.Models;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.AsiaGaming;
using Tgs.GameRounds.Common;

namespace Tgs.GameProviders.Core.UnitTest.AsiaGaming;

[TestFixture]
public class AsiaGamingRoundDetailGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<AsiaGamingRoundDetailGetService>.Instance;
        var fakeRoundService = Substitute.For<IRoundService>();
        var fakeGameRepository = Substitute.For<IGameRepository>();
        var fakeService = Substitute.For<IAsiaGamingService>();

        var configuration = new AsiaGamingConfiguration
        {
            ServiceEndpointUrl = new("http://game-ag"),
            AgentCode = "JH2_AGIN",
            ProductionId = "JH2",
            LockGameTypes = new[]
                   {
                        "ROU"
                    },
            DesKey = "jh2hnM6K",
            Md5Key = "jh2bKFbpLZuW",
            GameHistoryTypes =
                       new[]
                       {
                            new AsiaGamingGameHistoryType
                            {
                                GameTypes = new[]
                                {
                                    "BAC",
                                    "LBAC",
                                    "CBAC"
                                },
                                Descriptions = new Dictionary<string, string>
                                {
                                    {"P", "Player"},
                                    {"B", "Banker"}
                                }
                            },
                            new AsiaGamingGameHistoryType
                            {
                                GameTypes = new[]
                                {
                                    "DT"
                                },
                                Descriptions = new Dictionary<string, string>
                                {
                                    {"D", "Dragon"},
                                    {"T", "Tiger"}
                                }
                            },
                            new AsiaGamingGameHistoryType
                            {
                                GameTypes = new[]
                                {
                                    "NN"
                                },
                                Descriptions = new Dictionary<string, string>
                                {
                                    {"F", "First"},
                                    {"P1", "Player1"},
                                    {"P2", "Player2"},
                                    {"P3", "Player3"},
                                    {"B", "Banker"}
                                }
                            },
                            new AsiaGamingGameHistoryType
                            {
                                GameTypes = new[]
                                {
                                    "ZJH"
                                },
                                Descriptions = new Dictionary<string, string>
                                {
                                    {"D", "Dragon"},
                                    {"P", "Phoenix"}
                                }
                            },
                            new AsiaGamingGameHistoryType
                            {
                                GameTypes = new[]
                                {
                                    "BF"
                                },
                                Descriptions = new Dictionary<string, string>
                                {
                                    {"B", "BlackBull"},
                                    {"R", "RedBull"}
                                }
                            },
                            new AsiaGamingGameHistoryType
                            {
                                GameTypes = new[]
                                {
                                    "BJ",
                                    "SG"
                                },
                                Descriptions = new Dictionary<string, string>
                                {
                                    {"B", "Banker"},
                                    {"P1", "Player1"},
                                    {"P2", "Player2"},
                                    {"P3", "Player3"},
                                    {"P4", "Player4"},
                                    {"P5", "Player5"},
                                    {"P6", "Player6"},
                                    {"P7", "Player7"}
                                }
                            }
                       },
            LoginApiUrl = new("http://gi.aggdemo.com:81"),
            ApiUrl = new("https://swapi.etwlt.com"),
            OppositeBets = Array.Empty<OppositeBetSetting>()
        };

        var player = new PlayerInfo(
         Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
         "TGS",
         Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
         "TGS",
         Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
         "GPI",
         "4ucmqplxb029",
         "4ucmqplxb029",
         Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
         "CNY");

        var gameInfo = new GameInfo(
            GameId: 400001,
            GameCode: "AG_Deluxe_Baccarat",
            GameProviderId: Guid.Parse("629101fc-3b1c-4719-881b-dcf3c7263c15"),
            GameProviderCode: GameProviderType.AsiaGaming,
            GameExternalId: "AGIN_AGQ_BAC",
            Type: GameType.LiveDealer,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: "AG Deluxe Baccarat",
            SubGame: false,
            AvailableToBrand: true,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: null,
            EndpointPath: "/forwardGame.do?params={PARAS}&key={KEY}",
            RuleUrl: string.Empty,
            AvailableCurrencies: null,
            Localization: null,
            SubGameProviderCode: GameProviderType.AsiaGaming);

        var roundInfo = new RoundInfo(
            Guid.Parse("5566e31a-80ec-46eb-522d-08d9c07bf52d"),
            "GN0022160907S",
            Guid.Parse("629101fc-3b1c-4719-881b-dcf3c7263c15"),
            player.PlayerCode,
            gameInfo.GameId,
            player.DefaultWalletCurrencyCode,
            GameRoundStatus.Closed,
            Guid.NewGuid(),
            player.DefaultWalletId,
            DateTime.Now.AddMinutes(-1),
            DateTime.Now,
            DateTime.Now,
            player.BrandId,
            DateTime.Now.AddMinutes(-1),
            DateTime.Now,
            20m,
            20m,
            45m,
            null,
            20m,
            -25m,
            null,
            GameRoundClientStatus.Win,
            null,
            null,
            null,
            JsonSerializer.Serialize(new AsiaGamingGameHistory("P;H8;DJ:B;D10;D2")));

        var query = new AsiaGamingRoundDetailQuery
        {
            RoundId = roundInfo.RoundId,
            Language = "en-US"
        };

        var expected = new RoundDetailResult
        {
            Data = new RoundDetailDataBase[]
            {
                new RoundDetailDataJson
                {
                    Value = JsonSerializer.Serialize(new Dictionary<string, string>
                    {
                        ["Player"] = "♥8;♦J",
                        ["Banker"] = "♦10;♦2"
                    })
                }
            }
        };

        _ = fakeRoundService.GetAsync(query.RoundId).Returns(ValueTask.FromResult<RoundInfo?>(roundInfo));
        _ = fakeGameRepository.GetAsync(roundInfo.GameId).Returns(ValueTask.FromResult<GameInfo?>(gameInfo));
        _ = fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));

        var sut = new AsiaGamingRoundDetailGetService(
            fakeLogger,
            fakeRoundService,
            fakeGameRepository,
            fakeService);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        CollectionAssert.AreEqual(expected.Data, actual.Data);
    }
}
