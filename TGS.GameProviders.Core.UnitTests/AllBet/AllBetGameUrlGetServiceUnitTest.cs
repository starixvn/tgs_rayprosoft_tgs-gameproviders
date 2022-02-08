using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using TGS.GameProviders.Core.AllBet;
using TGS.GameProviders.Core.AllBet.ApplicationServices;
using TGS.GameProviders.Core.AllBet.Models;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.AllBet;

namespace TGS.GameProviders.Core.UnitTests.AllBet;

[TestFixture]
public class AllBetGameUrlGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<AllBetGameUrlGetService>.Instance;
        var fakeService = Substitute.For<IAllBetService>();
        var fakeApiClient = Substitute.For<IAllBetApiClient>();
        var fakePlayerService = Substitute.For<IAllBetGamePlayer>();
        var fakeAgentMapper = Substitute.For<IAllBetAgentMapper>();

        var gameUrl = new Uri("http://www.google.com");
        var gameProviderLanguageCode = "en-US";

        var agent = new AllBetAgent
        {
            PropertyId = "4222910",
            AgentId = "ygpg1a",
            CurrencyCode = "CNY",
            AllBetApiKey =
                "VO5ajdrJrayBQwaEjlMMVmYsME1n38q8Mo5COHKLdymAxP0TQ8Rmq2KdWwly6SlOPKbz4GBM0EDtnE+c6DtVrA==",
            PartnerApiKey =
                "wqRXtqmTssY6dsMb6y7janayZosdF2+KIOZx8G0UWeNOIl+B1+hn56y/unBBR2OUBhgyvxo7sdD9RQt5C9TDkg==",
            Suffix = "mkb"
        };

        var configuration = new AllBetConfiguration
        {
            ServiceEndpointUrl = new("http://game-ab"),
            ApiUrl = new("https://sw2.apidemo.net:8443/"),
            BrandMerchants = new AllBetBrandMerchant[]
            {
                new()
                {
                    BrandCode = "Default",
                    Agents = new[] { agent }
                }
            }
        };

        var gameModel = new GameInfo(
            1300001,
            GameStatus.Active,
            "AllBetLobby",
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
            "AB");

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

        var allBetPlayerId = $"{playerModel.PlayerCode}_{agent.Suffix}";

        var query = new AllBetGameUrlQuery
        {
            GameType = gameModel.GameType,
            GameId = gameModel.GameId,
            BrandCode = "GPI",
            PlayerCode = playerModel.PlayerCode,
            CurrencyCode = "CNY",
            LoginRecordId = Guid.NewGuid(),
            PlatformType = PlatformType.Desktop,
            Language = "en-US",
            IPAddress = "127.0.0.1",
            BackUrl = new("https://dev-t1.rpgp.cc/")
        };

        fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        fakeService.ToAllBetPlayerId(playerModel.PlayerCode, agent.Suffix).Returns(allBetPlayerId);

        fakeAgentMapper.GetAgent(configuration.BrandMerchants, query.BrandCode, query.CurrencyCode)
            .Returns(agent);

        fakeService.GetGameProviderLanguageAsync(query.Language)
            .Returns(ValueTask.FromResult(gameProviderLanguageCode));

        fakeApiClient
            .LoginAsync(
                configuration.ApiUrl,
                agent.PropertyId,
                agent.AllBetApiKey,
                new AllBetLoginRequest
                {
                    PlayerId = allBetPlayerId,
                    Language = gameProviderLanguageCode
                })
            .Returns(ValueTask.FromResult(new AllBetLoginResponse
            {
                ErrorCode = "OK",
                Data = new AllBetLoginData { Url = gameUrl.AbsoluteUri }
            }));

        var sut = new AllBetGameUrlGetService(
            fakeLogger,
            fakeService,
            fakeAgentMapper,
            fakePlayerService,
            fakeApiClient);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(gameUrl, actual.Url);
    }
}
