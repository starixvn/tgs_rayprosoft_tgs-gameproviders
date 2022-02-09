using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using Tgs.Core.Infrastructures.Security;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.AsiaGaming;
using TGS.GameProviders.Core.AsiaGaming.ApplicationServices;
using TGS.GameProviders.Core.AsiaGaming.Models;
using TGS.GameProviders.Core.BetConstruct;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.AsiaGaming;

namespace TGS.GameProviders.Client.IntegrationTests
{
    [TestFixture]
    public class GameProvidersClientUnitTest
    {
        [Test]
        public async Task Test_GameProvidersClient_GameUrlByAsiaGamingAsync()
        {
            var fakeService = Substitute.For<IAsiaGamingService>();
            var fakePlayerService = Substitute.For<IAsiaGamingGamePlayer>();
            var fakeSessionService = Substitute.For<IAsiaGamingGameSession>();
            var fakeUtility = Substitute.For<IAsiaGamingUtility>();

            await using var web = new TestGameProvidersApplication(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddTransient(_ => fakeService);
                    services.AddTransient(_ => fakePlayerService);
                    services.AddTransient(_ => fakeSessionService);

                    services.AddFakeNatsMessageQueue();
                });
            });

            var gameProviderLanguageCode = "3";  // en-us
            var gameProviderCurrencyCode = "CNY";
            var domain = "http://gci.aggdemo.com:81";

            var queryString = "queryString";
            var encodeParas = "encodeParas";
            var key = "key";

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
                Core.GameStatus.Active,
                "asiagaminglobby",
                null,
                "/forwardGame.do?params={PARAS}&key={KEY}",
                null,
                Guid.Parse("629101fc-3b1c-4719-881b-dcf3c7263c15"),
                Core.GameType.LiveDealerLobby,
                Core.GamePlatformType.DesktopAndMobile,
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
                Core.PlayerStatus.Enabled);

            var query = new AsiaGamingGameUrlQuery
            {
                GameType = gameInfo.GameType,
                GameId = gameInfo.GameId,
                BrandCode = "GPI",
                PlayerCode = playerInfo.PlayerCode,
                CurrencyCode = "CNY",
                LoginRecordId = Guid.NewGuid(),
                PlatformType = Core.PlatformType.Desktop,
                Language = "en-US",
                IPAddress = "127.0.0.1",
                BackUrl = new("https://dev-t1.rpgp.cc/"),
                PlayerBalance = 1000m
            };

            fakeService
                .GetSettingConfigurationAsync()
                .Returns(ValueTask.FromResult(configuration));

            fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
            fakeService.GetGameProviderIdAsync().Returns(ValueTask.FromResult(gameInfo.GameProviderId));
            fakeService.GetGameAsync(gameInfo.GameId).Returns(ValueTask.FromResult(gameInfo));

            fakeService
                .GetGameProviderCurrencyCodeAsync(gameInfo.GameProviderId, query.CurrencyCode)
                .Returns(ValueTask.FromResult(gameProviderCurrencyCode));

            fakeService.GetGameProviderLanguageCodeAsync(gameInfo.GameProviderId, query.Language)
                .Returns(ValueTask.FromResult(gameProviderLanguageCode));

            fakeService.GetBrandIdAsync(playerInfo.BrandCode).Returns(ValueTask.FromResult(playerInfo.BrandId));
            fakeService
                .GetGameDomainAsync(playerInfo.BrandId, gameInfo.GameProviderId, query.PlatformType)
                .Returns(ValueTask.FromResult(domain));

            fakePlayerService.CheckOrCreateAccountAsync(configuration, playerInfo.PlayerCode, gameProviderCurrencyCode)
                .Returns(ValueTask.FromResult(true));

            fakeSessionService
                .CreateGameSessionAsync(
                    configuration,
                    playerInfo.PlayerCode,
                    query.LoginRecordId,
                    query.PlayerBalance)
                .Returns(ValueTask.FromResult(true));

            fakeUtility.GetQueryString(
                    Arg.Is<AsiaGamingGameUrlRequest>(x =>
                        x.AgentCode == configuration.AgentCode &&
                        x.PlayerCode == playerInfo.PlayerCode &&
                        x.Lang == gameProviderLanguageCode &&
                        x.GameId == gameInfo.GameExternalId &&
                        x.Currency == gameProviderCurrencyCode),
                    Arg.Any<string>())
                .Returns(queryString);

            fakeUtility.DesEncrypt(queryString, configuration.DesKey).Returns(encodeParas);
            fakeUtility.Md5Hash($"{encodeParas}{configuration.Md5Key}").Returns(key);

            var httpClient = web.CreateClient();

            var sut = new GameProvidersClient(httpClient);

            var actual = await sut.GameUrlByAsiaGamingAsync(
                "AG",
                (Domains.GameType)query.GameType,
                query.GameId.Value,
                query.BrandCode,
                query.PlayerCode,
                query.CurrencyCode,
                query.LoginRecordId,
                (Domains.PlatformType)query.PlatformType,
                query.Language,
                query.IPAddress,
                query.BackUrl,
                query.PlayerBalance);

            Assert.IsNotNull(actual);
        }

        [Test]
        public async Task Test_GameProvidersClient_GameUrlByBetConstructAsync()
        {
            var fakeTokenProvider = Substitute.For<ITokenProvider>();
            var fakeBetConstructService = Substitute.For<IBetConstructService>();

            await using var web = new TestGameProvidersApplication(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton(fakeTokenProvider);
                    services.AddSingleton(fakeBetConstructService);
                    services.AddFakeNatsMessageQueue();
                });
            });

            fakeBetConstructService
                .GetSettingConfigurationAsync()
                .Returns(ValueTask.FromResult(new BetConstructConfiguration { PartnerId = "partnerId" }));

            var playerCode = "4ucmqplxb029";
            fakeTokenProvider
                .EncryptAsync(playerCode)
                .Returns("ZYE5oSH5I1hzmrgnQnh3R6");

            var httpClient = web.CreateClient();

            var sut = new GameProvidersClient(httpClient);

            var gameProviderCode = "BC";
            var gameType = Domains.GameType.Sportsbook;
            var gameId = 2100001;
            var brandCode = "GPI";
            var currencyCode = "CNY";
            var loginRecordId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            var platformType = Domains.PlatformType.Desktop;
            var language = "zh-CN";
            var ipAddress = "127.0.0.1";
            var lobbyDomain = "https://devsportsbook.srvttt.com";

            var actual = await sut.GameUrlByBetConstructAsync(
                gameProviderCode,
                gameType,
                gameId,
                brandCode,
                playerCode,
                currencyCode,
                loginRecordId,
                platformType,
                language,
                ipAddress,
                null,
                lobbyDomain);

            Assert.IsNotNull(actual);
        }
    }
}
