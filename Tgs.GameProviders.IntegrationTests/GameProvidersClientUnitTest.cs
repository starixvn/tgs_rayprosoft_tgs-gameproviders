using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Tgs.Core.Infrastructures.Security;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.AsiaGaming;
using Tgs.GameProviders.Core.AsiaGaming.ApplicationServices;
using Tgs.GameProviders.Core.AsiaGaming.Models;
using Tgs.GameProviders.Core.BetConstruct;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.AsiaGaming;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.IntegrationTests;

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

        var web = new TestGameProvidersApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                _ = services.AddTransient(_ => fakeService);
                _ = services.AddTransient(_ => fakePlayerService);
                _ = services.AddTransient(_ => fakeSessionService);

                _ = services.AddFakeNatsMessageQueue();
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

        var gameModel = new GameModel(
            400001,
            Guid.Parse("629101fc-3b1c-4719-881b-dcf3c7263c15"),
            "AG",
            "asiagaminglobby",
            GameStatus.Active,
            null,
            "/forwardGame.do?params={PARAS}&key={KEY}",
            null,
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

        var player = new PlayerModel(
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
            GameType = gameModel.GameType,
            GameId = gameModel.GameId,
            BrandCode = "GPI",
            PlayerCode = player.PlayerCode,
            CurrencyCode = "CNY",
            LoginRecordId = Guid.NewGuid(),
            PlatformType = Core.PlatformType.Desktop,
            Language = "en-US",
            IPAddress = "127.0.0.1",
            BackUrl = new("https://dev-t1.rpgp.cc/"),
            PlayerBalance = 1000m
        };

        _ = fakeService
            .GetSettingConfigurationAsync()
            .Returns(ValueTask.FromResult(configuration));

        _ = fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        _ = fakeService.GetGameProviderIdAsync().Returns(ValueTask.FromResult(gameModel.GameProviderId));
        _ = fakeService.GetGameAsync(gameModel.GameId).Returns(ValueTask.FromResult(gameModel));

        _ = fakeService
            .GetGameProviderCurrencyCodeAsync(gameModel.GameProviderId, query.CurrencyCode)
            .Returns(ValueTask.FromResult(gameProviderCurrencyCode));

        _ = fakeService.GetGameProviderLanguageCodeAsync(gameModel.GameProviderId, query.Language)
            .Returns(ValueTask.FromResult(gameProviderLanguageCode));

        _ = fakeService.GetBrandIdAsync(player.BrandCode).Returns(ValueTask.FromResult(player.BrandId));
        _ = fakeService
            .GetGameDomainAsync(player.BrandId, gameModel.GameProviderId, query.PlatformType)
            .Returns(ValueTask.FromResult(domain));

        _ = fakePlayerService.CheckOrCreateAccountAsync(configuration, player.PlayerCode, gameProviderCurrencyCode)
            .Returns(ValueTask.FromResult(true));

        _ = fakeSessionService
            .CreateGameSessionAsync(
                configuration,
                player.PlayerCode,
                query.LoginRecordId,
                query.PlayerBalance)
            .Returns(ValueTask.FromResult(true));

        _ = fakeUtility.GetQueryString(
                Arg.Is<AsiaGamingGameUrlRequest>(x =>
                    x.AgentCode == configuration.AgentCode &&
                    x.PlayerCode == player.PlayerCode &&
                    x.Lang == gameProviderLanguageCode &&
                    x.GameId == gameModel.GameExternalId &&
                    x.Currency == gameProviderCurrencyCode),
                Arg.Any<string>())
            .Returns(queryString);

        _ = fakeUtility.DesEncrypt(queryString, configuration.DesKey).Returns(encodeParas);
        _ = fakeUtility.Md5Hash($"{encodeParas}{configuration.Md5Key}").Returns(key);

        var fakeLoggingHandler = new LoggingHandler(web.Services.GetRequiredService<ILogger<LoggingHandler>>());

        var httpClient = web.CreateDefaultClient(new System.Net.Http.DelegatingHandler[] { fakeLoggingHandler });

        var sut = new GameProvidersClient(httpClient);

        var actual = await sut.GameUrlByAsiaGamingAsync(
            (GameProvidersClientGameType)query.GameType,
            query.GameId.Value,
            query.BrandCode,
            query.PlayerCode,
            query.CurrencyCode,
            query.LoginRecordId,
            (GameProvidersClientPlatformType)query.PlatformType,
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

        var web = new TestGameProvidersApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                _ = services.AddSingleton(fakeTokenProvider);
                _ = services.AddSingleton(fakeBetConstructService);
                _ = services.AddFakeNatsMessageQueue();
            });
        });

        _ = fakeBetConstructService
            .GetSettingConfigurationAsync()
            .Returns(ValueTask.FromResult(new BetConstructConfiguration { PartnerId = "partnerId" }));

        var playerCode = "4ucmqplxb029";
        _ = fakeTokenProvider
            .EncryptAsync(playerCode)
            .Returns("ZYE5oSH5I1hzmrgnQnh3R6");

        var fakeLoggingHandler = new LoggingHandler(web.Services.GetRequiredService<ILogger<LoggingHandler>>());

        var httpClient = web.CreateDefaultClient(new System.Net.Http.DelegatingHandler[] { fakeLoggingHandler });

        var sut = new GameProvidersClient(httpClient);

        var gameType = GameProvidersClientGameType.Sportsbook;
        var gameId = 2100001;
        var brandCode = "GPI";
        var currencyCode = "CNY";
        var loginRecordId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
        var platformType = GameProvidersClientPlatformType.Desktop;
        var language = "zh-CN";
        var ipAddress = "127.0.0.1";
        var lobbyDomain = "https://devsportsbook.srvttt.com";

        var actual = await sut.GameUrlByBetConstructAsync(
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

    [Test]
    public async Task Test_GameProvidersClient_GamePlatformLoginBySM01Async()
    {
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeBrandRepository = Substitute.For<IBrandRepository>();

        var web = new TestGameProvidersApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                _ = services.AddTransient(_ => fakeGameProviderRepository);
                _ = services.AddTransient(_ => fakePlayerRepository);
                _ = services.AddTransient(_ => fakeBrandRepository);
            });
        });

        var gameProviderId = Guid.Parse("ba2997d9-b856-4589-91ae-5060a952cc3c");
        var playerCode = "4ucmqplxb029";
        var gameProviderConfigurationId = Guid.Parse("6bd1773a-6ba3-4087-83dd-737d0179831a");

        _ = fakeGameProviderRepository.GetGameProviderIdAsync(GameProviderType.SM01).Returns(ValueTask.FromResult(gameProviderId));

        var player = new PlayerModel(
            Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
            "TGS",
            Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
            "TGS",
            BrandId: Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
            "GPI",
            PlayerCode: "4ucmqplxb029",
            "4ucmqplxb029",
            Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
            "CNY",
            PlayerStatus.Enabled);

        _ = fakePlayerRepository.GetAsync(playerCode).Returns(player);

        _ = fakeBrandRepository.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).Returns(ValueTask.FromResult(gameProviderConfigurationId));

        var config = new SM01Configuration
        {
            ApiUrl = new Uri("https://cn.ucfclive.com"),
            MerchantId = "SQjCxm2cX5bahBta",
            MerchantKey = "XGKGkcmYNnfex8AecKzCEbD4GMExA2cb",
        };

        _ = fakeGameProviderRepository.GetSM01Configuration(gameProviderConfigurationId).Returns(ValueTask.FromResult(config));

        var fakeLoggingHandler = new LoggingHandler(web.Services.GetRequiredService<ILogger<LoggingHandler>>());

        var httpClient = web.CreateDefaultClient(new System.Net.Http.DelegatingHandler[] { fakeLoggingHandler });

        var sut = new GameProvidersClient(httpClient);

        var actual = await sut.GamePlatformLoginBySM01Async(
            playerCode);

        Assert.IsNotNull(actual);
    }
}
