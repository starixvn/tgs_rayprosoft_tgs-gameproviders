using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using TGS.GameProviders.Core.AsiaGaming;
using TGS.GameProviders.Core.AsiaGaming.ApplicationServices;
using TGS.GameProviders.Core.AsiaGaming.Models;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Repositories.Models.GameProvider;
using TGS.GameProviders.Core.Repositories.Models.Rounds;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.RoundDetail;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.AsiaGaming;

namespace TGS.GameProviders.Core.UnitTests.AsiaGaming;

[TestFixture]
public class AsiaGamingRoundDetailGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<AsiaGamingRoundDetailGetService>.Instance;
        var fakeRoundRepository = Substitute.For<IRoundRepository>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeGameService = Substitute.For<IGameService>();
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

        var playerModel = new PlayerInfo(
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

        var gameProviderModel = new GameProviderModel(
            Guid.Parse("629101fc-3b1c-4719-881b-dcf3c7263c15"),
            GameProviderStatus.Active,
            null,
            true,
            true,
            true,
            10,
            5,
            13,
            false,
            "AG",
            "AsiaGaming",
            null);

        var gameModel = new GameInfo(
            400013,
            GameStatus.Active,
            "AGIN_AGQ_BAC",
            null,
            "/forwardGame.do?params={PARAS}&key={KEY}",
            null,
            Guid.Parse("629101fc-3b1c-4719-881b-dcf3c7263c15"),
            GameType.LiveDealer,
            GamePlatformType.DesktopAndMobile,
            false,
            true,
            false,
            true,
            null,
            "AG_Deluxe_Baccarat",
            "AG Deluxe Baccarat",
            null,
            "AG");

        var roundModel = new RoundModel(
            Guid.Parse("5566e31a-80ec-46eb-522d-08d9c07bf52d"),
            "GN0022160907S",
            gameProviderModel.GameProviderId,
            playerModel.PlayerCode,
            gameModel.GameId,
            playerModel.DefaultWalletCurrencyCode,
            RoundStatus.Closed,
            Guid.NewGuid(),
            playerModel.DefaultWalletId,
            DateTime.Now.AddMinutes(-1),
            DateTime.Now,
            DateTime.Now,
            playerModel.BrandId,
            DateTime.Now.AddMinutes(-1),
            DateTime.Now,
            20m,
            20m,
            45m,
            null,
            20m,
            -25m,
            null,
            ClientRoundStatus.Win,
            null,
            null,
            null,
            JsonSerializer.Serialize(new AsiaGamingGameHistory("P;H8;DJ:B;D10;D2")));

        var query = new AsiaGamingRoundDetailQuery
        {
            RoundId = roundModel.RoundId,
            Language = "en-US"
        };

        var expected = new RoundDetailResult
        {
            GameProviderName = gameProviderModel.Name,
            GameName = gameModel.Name,
            ExternalRoundId = roundModel.ExternalRoundId,
            BetAmount = roundModel.BetAmount,
            Turnover = roundModel.Turnover,
            Winloss = roundModel.WinLoss,
            GameResult = roundModel.ClientStatus.ToString(),
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

        _ = fakeRoundRepository.GetAsync(query.RoundId).Returns(Task.FromResult(roundModel));
        _ = fakeGameProviderRepository.GetAsync(roundModel.GameProviderId).Returns(ValueTask.FromResult(gameProviderModel));
        _ = fakeGameService.GetCacheAsync(roundModel.GameId).Returns(ValueTask.FromResult(gameModel));
        _ = fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));

        var sut = new AsiaGamingRoundDetailGetService(
            fakeLogger,
            fakeRoundRepository,
            fakeGameProviderRepository,
            fakeGameService,
            fakeService);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(expected.GameProviderName, actual.GameProviderName);
        Assert.AreEqual(expected.GameName, actual.GameName);
        Assert.AreEqual(expected.ExternalRoundId, actual.ExternalRoundId);
        Assert.AreEqual(expected.BetAmount, actual.BetAmount);
        Assert.AreEqual(expected.Turnover, actual.Turnover);
        Assert.AreEqual(expected.Winloss, actual.Winloss);
        Assert.AreEqual(expected.GameResult, actual.GameResult);
    }
}
