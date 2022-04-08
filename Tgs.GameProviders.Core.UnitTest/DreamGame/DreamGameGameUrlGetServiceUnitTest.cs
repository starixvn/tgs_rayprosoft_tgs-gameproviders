using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.DreamGame;
using Tgs.GameProviders.Core.DreamGame.ApplicationServices;
using Tgs.GameProviders.Core.DreamGame.Models;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.DreamGame;
using Tgs.GameProviders.QueryServer.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.UnitTest.DreamGame;

[TestFixture]
public class DreamGameGameUrlGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeService = Substitute.For<IDreamGameService>();
        var fakeApiClient = Substitute.For<IDreamGameApiClient>();
        var fakeCreatePlayerService = Substitute.For<IGetService<DreamGameCreatePlayerQuery, int>>();

        var gameUrl = new Uri("http://www.google.com");

        var configuration = new DreamGameConfiguration
        {
            ServiceEndpointUrl = new("http://game-dg"),
            ApiUrl = new("http://api.dg99web.com"),
            VideoGameApiUrl = new("https://ag.dg66.info"),
            ApiKey = "974e8e9a072e4ad59a9e3f43a06ff9ae",
            AgentName = "DGTE0105CO",
            PlayerPassword = "TgsP@ssw0rd",
            BetLimits = new[] {
                new DreamGameBetLimit(){
                    Currency="CNY",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="EUR",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="GBP",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="HKD",
                    TableLimit="G"
                },
                new DreamGameBetLimit(){
                    Currency="IDR_1000",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="INR",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="JPY",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="KRW",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="KRW",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="MYR",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="PHP",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="SGD",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="THB",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="USD",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="VND_1000",
                    TableLimit="A"
                },
            }
        };

        var gameInfo = new GameInfo(
            2500001,
            GameStatus.Active,
            "DreamGameLobby",
            null,
            null,
            null,
            Guid.Parse("579d6da3-b0ed-4e3f-bd4a-434a98efbd68"),
            GameType.LiveDealerLobby,
            GamePlatformType.Desktop,
            false,
            true,
            false,
            true,
            null,
            "Lobby",
            "Lobby",
            null,
            "DG");

        var query = new DreamGameGameUrlQuery
        {
            GameType = gameInfo.GameType,
            GameId = gameInfo.GameId,
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            LoginRecordId = Guid.NewGuid(),
            PlatformType = PlatformType.Desktop,
            Language = "en-US",
            IPAddress = "127.0.0.1",
            BackUrl = new("https://dev-t1.rpgp.cc/")
        };
        var gameProviderCultureCode = "en";
        var gameProviderCurrencyCode = "CNY";
        var loginResult = new LoginResult(true, 0, gameUrl);
        fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        fakeService.GetGameProviderLanguageCodeAsync(query.Language).Returns(gameProviderCultureCode);
        fakeService.GetGameProviderCurrencyCodeAsync(query.CurrencyCode).Returns(gameProviderCurrencyCode);

        fakeApiClient.LoginAsync
            (configuration.ApiUrl,
                 configuration.ApiKey,
                 configuration.AgentName,
                 query.PlayerCode,
                 configuration.PlayerPassword,
                 query.PlatformType,
                 gameProviderCultureCode)
            .Returns(loginResult);

        var sut = new DreamGameGameUrlGetService(
            fakeService,
            fakeApiClient,
            fakeCreatePlayerService);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(gameUrl, actual.Url);
    }
}
