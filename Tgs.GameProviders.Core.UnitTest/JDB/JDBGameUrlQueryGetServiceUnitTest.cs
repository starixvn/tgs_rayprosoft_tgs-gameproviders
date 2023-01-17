using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Core.JDB;
using Tgs.GameProviders.Core.JDB.ApplicationServices;
using Tgs.GameProviders.Core.JDB.Models;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.JDB;

namespace Tgs.GameProviders.Core.UnitTest.JDB;

[TestFixture]
public class JDBGameUrlQueryGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeGameRepository = Substitute.For<IGameRepository>();
        var fakeService = Substitute.For<IJDBService>();
        var fakeAgentMapper = Substitute.For<IJDBAgentMapper>();
        var fakeApiClient = Substitute.For<IJDBApiClient>();

        var gameUrl = new Uri("http://www.google.com");
        var gameProviderLanguageCode = "en";

        var agent = new JDBAgent
        {
            Id = "wgvb3rmb",
            CurrencyCode = "CNY"
        };

        var configuration = new JDBConfiguration
        {
            ServiceEndpointUrl = new("http://game-jdb"),
            ApiUrl = new("http://api.jdb711.com"),
            AESIv = "756edb2fc85fe908",
            AESKey = "4f09f467e6a8fb9f",
            ClientDomain = "VB3",
            TimezoneHour = 8,
            MoreGame = false,
            Agents = new[] { agent }
        };

        var gameInfo = new GameInfo(
            GameId: 1,
            GameCode: "GoLaiFu",
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: GameProviderType.JDB,
            GameExternalId: "0_14034",
            Type: GameType.Slot,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: "GoLaiFu",
            SubGame: true,
            AvailableToBrand: false,
            HasTurnoverOnTieBet: true,
            Html5: true,
            Description: null,
            EndpointPath: null,
            RuleUrl: null,
            AvailableCurrencies: null,
            Localization: null,
            SubGameProviderCode: GameProviderType.JDB);

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

        var query = new JDBGameUrlQuery
        {
            GameType = gameInfo.Type,
            GameId = gameInfo.GameId,
            BrandCode = "GPI",
            PlayerCode = player.PlayerCode,
            CurrencyCode = "CNY",
            LoginRecordId = Guid.NewGuid(),
            PlatformType = PlatformType.Desktop,
            Language = "en-US",
            IPAddress = "127.0.0.1",
            BackUrl = new("https://dev-t1.rpgp.cc/")
        };

        fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        fakeService.GetGameProviderLanguageAsync(gameInfo.GameProviderId, query.Language)
            .Returns(ValueTask.FromResult(gameProviderLanguageCode));

        fakeAgentMapper.GetAgent(configuration.Agents, player.DefaultWalletCurrencyCode)
            .Returns(agent);

        fakePlayerRepository.GetAsync(query.PlayerCode).Returns(ValueTask.FromResult(player));
        fakeGameRepository.GetAsync(query.GameId.Value).Returns(ValueTask.FromResult(gameInfo));

        var items = gameInfo.GameExternalId.Split('_');

        fakeApiClient
            .GetTokenAsync(
                configuration.ApiUrl,
                configuration.ClientDomain,
                configuration.AESIv,
                configuration.AESKey,
                Arg.Is<JDBGetTokenRequest>(x
                    => x.Parent == agent.Id &&
                       x.UserId == player.PlayerCode &&
                       x.Language == gameProviderLanguageCode &&
                       x.GameType == items[0] &&
                       x.MachineType == items[1] &&
                       x.MoreGame == 0))
            .Returns(ValueTask.FromResult(
                new JDBGetTokenResponse
                {
                    Status = "0000",
                    Path = gameUrl.AbsoluteUri
                }));

        var sut = new JDBGameUrlGetService(
            fakePlayerRepository,
            fakeGameRepository,
            fakeService,
            fakeAgentMapper,
            fakeApiClient);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(gameUrl, actual.Url);
    }
}
