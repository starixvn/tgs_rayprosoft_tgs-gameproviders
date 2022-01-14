using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.Core.DependencyInjection;
using NUnit.Framework;
using NUnit.Framework.Internal;
using TGS.GameProviders.Core.JDB;
using TGS.GameProviders.Core.JDB.ApplicationServices;
using TGS.GameProviders.Core.JDB.Models;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Repositories.Models.Game;
using TGS.GameProviders.Core.Repositories.Models.Player;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.JDB;

namespace TGS.GameProviders.Core.UnitTests.JDB;

[TestFixture]
public class JDBGameUrlQueryGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<JDBGameUrlGetService>.Instance;
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

        var gameModel = new GameModel(
            700010,
            GameStatus.Active,
            "0_14034",
            null,
            null,
            null,
            Guid.Parse("65cc2057-cb52-44aa-8bff-dbb3d9ebdddc"),
            GameType.Slot,
            GamePlatformType.DesktopAndMobile,
            false,
            true,
            false,
            true,
            null,
            "GoLaiFu",
            "GoLaiFu",
            null,
            "JDB");

        var playerModel = new PlayerModel(
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

        var query = new JDBGameUrlQuery
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
        fakeService.GetGameProviderLanguageAsync(gameModel.GameProviderId, query.Language)
            .Returns(ValueTask.FromResult(gameProviderLanguageCode));

        fakeAgentMapper.GetAgent(configuration.Agents, playerModel.DefaultWalletCurrencyCode)
            .Returns(agent);

        fakePlayerRepository.GetAsync(query.PlayerCode).Returns(Task.FromResult(playerModel));
        fakeGameRepository.GetAsync(query.GameId.Value).Returns(Task.FromResult(gameModel));

        var items = gameModel.GameExternalId.Split('_');

        fakeApiClient
            .GetTokenAsync(
                configuration.ApiUrl,
                configuration.ClientDomain,
                configuration.AESIv,
                configuration.AESKey,
                Arg.Is<JDBGetTokenRequest>(x
                    => x.Parent == agent.Id &&
                       x.UserId == playerModel.PlayerCode &&
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
            fakeLogger,
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