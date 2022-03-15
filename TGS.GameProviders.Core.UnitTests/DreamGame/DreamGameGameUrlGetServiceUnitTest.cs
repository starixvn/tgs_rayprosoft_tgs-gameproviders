using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.QueryServer.Models;
using TGS.GameProviders.Core.DreamGame;
using TGS.GameProviders.Core.DreamGame.ApplicationServices;
using TGS.GameProviders.Core.DreamGame.Models;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.UnitTests.DreamGame;

[TestFixture]
public class DreamGameGameUrlGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeService = Substitute.For<IDreamGameService>();
        var fakeApiClient = Substitute.For<IDreamGameApiClient>();

        var gameUrl = new Uri("http://www.google.com");

        var configuration = new DreamGameConfiguration
        {
            ServiceEndpointUrl = new("http://game-dg"),
            ApiUrl = new("http://api.dg99web.com"),
            VideoGameApiUrl = new("https://ag.dg66.info"),
            ApiKey = "974e8e9a072e4ad59a9e3f43a06ff9ae",
            AgentName = "DGTE0105CO",
            PlayerPassword = "TgsP@ssw0rd"
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
            fakeApiClient);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(gameUrl, actual.Url);
    }
}
