using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.QueryServer.Models;
using TGS.GameProviders.Core.AsiaGaming;
using TGS.GameProviders.Core.AsiaGaming.ApplicationServices;
using TGS.GameProviders.Core.AsiaGaming.Models;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.AsiaGaming;

namespace TGS.GameProviders.Core.UnitTests.AsiaGaming;

[TestFixture]
public class AsiaGamingGameUrlGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<AsiaGamingGameUrlGetService>.Instance;
        var fakeService = Substitute.For<IAsiaGamingService>();
        var fakeUtility = Substitute.For<IAsiaGamingUtility>();
        var fakePlayerService = Substitute.For<IAsiaGamingGamePlayer>();
        var fakeSessionService = Substitute.For<IAsiaGamingGameSession>();

        var gameProviderLanguageCode = "3";  // en-us
        var gameProviderCurrencyCode = "CNY";
        var domain = "http://gci.aggdemo.com:81";

        var queryString = "queryString";
        var encodeParas = "encodeParas";
        var key = "key";

        var gameUrl = new Uri($"{domain}/forwardGame.do?params={encodeParas}&key={key}");

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

        var gameInfo = new GameInfo(
            400001,
            GameStatus.Active,
            "asiagaminglobby",
            null,
            "/forwardGame.do?params={PARAS}&key={KEY}",
            null,
            Guid.Parse("629101fc-3b1c-4719-881b-dcf3c7263c15"),
            GameType.LiveDealerLobby,
            GamePlatformType.DesktopAndMobile,
            false,
            true,
            false,
            true,
            null,
            "Lobby",
            "Lobby",
            null,
            "AG");

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

        var query = new AsiaGamingGameUrlQuery
        {
            GameType = gameInfo.GameType,
            GameId = gameInfo.GameId,
            BrandCode = "GPI",
            PlayerCode = playerInfo.PlayerCode,
            CurrencyCode = "CNY",
            LoginRecordId = Guid.NewGuid(),
            PlatformType = PlatformType.Desktop,
            Language = "en-US",
            IPAddress = "127.0.0.1",
            BackUrl = new("https://dev-t1.rpgp.cc/"),
            PlayerBalance = 1000m
        };

        fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        fakeService.GetGameProviderIdAsync().Returns(ValueTask.FromResult(gameInfo.GameProviderId));
        fakeService.GetGameAsync(query.GameId.Value).Returns(ValueTask.FromResult(gameInfo));

        fakeService
            .GetGameProviderCurrencyCodeAsync(gameInfo.GameProviderId, query.CurrencyCode)
            .Returns(ValueTask.FromResult(gameProviderCurrencyCode));

        fakeService.GetGameProviderLanguageCodeAsync(gameInfo.GameProviderId, query.Language)
            .Returns(ValueTask.FromResult(gameProviderLanguageCode));

        fakeService.GetBrandIdAsync(query.BrandCode).Returns(ValueTask.FromResult(playerInfo.BrandId));
        fakeService.GetGameDomainAsync(playerInfo.BrandId, gameInfo.GameProviderId, query.PlatformType)
            .Returns(ValueTask.FromResult(domain));

        fakePlayerService.CheckOrCreateAccountAsync(configuration, query.PlayerCode, gameProviderCurrencyCode)
            .Returns(ValueTask.FromResult(true));

        fakeSessionService
            .CreateGameSessionAsync(
                configuration,
                query.PlayerCode,
                query.LoginRecordId,
                query.PlayerBalance)
            .Returns(ValueTask.FromResult(true));

        fakeUtility.GetQueryString(
                Arg.Is<AsiaGamingGameUrlRequest>(x =>
                    x.AgentCode == configuration.AgentCode &&
                    x.PlayerCode == query.PlayerCode &&
                    x.Lang == gameProviderLanguageCode &&
                    x.GameId == gameInfo.GameExternalId &&
                    x.ReturnUrl == query.BackUrl.AbsoluteUri &&
                    x.Currency == gameProviderCurrencyCode),
                Arg.Any<string>())
            .Returns(queryString);

        fakeUtility.DesEncrypt(queryString, configuration.DesKey).Returns(encodeParas);
        fakeUtility.Md5Hash($"{encodeParas}{configuration.Md5Key}").Returns(key);

        var sut = new AsiaGamingGameUrlGetService(
            fakeLogger,
            fakeService,
            fakePlayerService,
            fakeSessionService,
            fakeUtility);

        await sut.GetAsync(query).ConfigureAwait(false);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(gameUrl, actual.Url);
    }
}
