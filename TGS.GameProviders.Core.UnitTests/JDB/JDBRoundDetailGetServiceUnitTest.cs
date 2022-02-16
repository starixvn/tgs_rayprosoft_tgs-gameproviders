using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.QueryServer.Models;
using TGS.GameProviders.Core.JDB;
using TGS.GameProviders.Core.JDB.ApplicationServices;
using TGS.GameProviders.Core.JDB.Models;
using TGS.GameProviders.Domains.RoundDetail;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.JDB;

namespace TGS.GameProviders.Core.UnitTests.JDB;

[TestFixture]
public class JDBRoundDetailGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<JDBRoundDetailGetService>.Instance;
        var fakePlayerService = Substitute.For<IPlayerService>();
        var fakeRoundService = Substitute.For<IRoundService>();
        var fakeGameProviderService = Substitute.For<IGameProviderService>();
        var fakeGameService = Substitute.For<IGameService>();
        var fakeService = Substitute.For<IJDBService>();
        var fakeAgentMapper = Substitute.For<IJDBAgentMapper>();
        var fakeApiClient = Substitute.For<IJDBApiClient>();

        var gameProviderLanguageCode = "en";
        var historyUrl = "https://www.google.com";

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

        var gameProviderInfo = new GameProviderInfo(
            Guid.Parse("65cc2057-cb52-44aa-8bff-dbb3d9ebdddc"),
            GameProviderStatus.Active,
            null,
            true,
            true,
            true,
            10,
            5,
            7,
            false,
            "JDB",
            "JDB",
            null);

        var gameInfo = new GameInfo(
            700010,
            GameStatus.Active,
            "0_14034",
            null,
            null,
            null,
            gameProviderInfo.GameProviderId,
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

        var roundInfo = new RoundInfo(
            Guid.Parse("015dc0ab-eecc-49da-299e-08d9d18bf7ef"),
            "5250172280339",
            gameProviderInfo.GameProviderId,
            playerInfo.PlayerCode,
            gameInfo.GameId,
            playerInfo.DefaultWalletCurrencyCode,
            RoundStatus.Closed,
            Guid.NewGuid(),
            playerInfo.DefaultWalletId,
            DateTime.Now.AddMinutes(-1),
            DateTime.Now,
            DateTime.Now,
            playerInfo.BrandId,
            DateTime.Now.AddMinutes(-1),
            DateTime.Now,
            20m,
            20m,
            45m,
            null,
            20m,
            -25m,
            null,
            RoundClientStatus.Win,
            null,
            null,
            null,
            null);

        var query = new JDBRoundDetailQuery
        {
            RoundId = roundInfo.RoundId,
            Language = "en-US"
        };

        var expected = new RoundDetailResult
        {
            Data = new RoundDetailDataBase[]
            {
                new RoundDetailDataUrl
                {
                    Url = historyUrl
                }
            }
        };

        fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        fakeService.GetGameProviderLanguageAsync(roundInfo.GameProviderId, query.Language)
            .Returns(ValueTask.FromResult(gameProviderLanguageCode));

        fakeAgentMapper.GetAgent(configuration.Agents, playerInfo.DefaultWalletCurrencyCode)
            .Returns(agent);

        fakeRoundService.GetAsync(query.RoundId).Returns(ValueTask.FromResult<RoundInfo?>(roundInfo));
        fakePlayerService.GetCacheAsync(roundInfo.PlayerCode).Returns(Task.FromResult(playerInfo));
        fakeGameProviderService.GetAsync(roundInfo.GameProviderId).Returns(ValueTask.FromResult(gameProviderInfo));
        fakeGameService.GetCacheAsync(roundInfo.GameId).Returns(ValueTask.FromResult<GameInfo?>(gameInfo));

        var items = gameInfo.GameExternalId.Split('_');

        fakeApiClient
            .GetBetHistoryUrlAsync(
                configuration.ApiUrl,
                configuration.ClientDomain,
                configuration.AESIv,
                configuration.AESKey,
                Arg.Is<JDBBetHistoryRequest>(x
                    => x.Parent == agent.Id &&
                       x.UserId == playerInfo.PlayerCode &&
                       x.Language == gameProviderLanguageCode &&
                       x.GameType == int.Parse(items[0]) &&
                       x.SequenceNo == long.Parse(roundInfo.ExternalRoundId)))
            .Returns(ValueTask.FromResult(
                new JDBBetHistoryResponse
                {
                    Status = "0000",
                    Data = new JDBBetHistoryPageUrl[]
                    {
                        new()
                        {
                            Path = historyUrl
                        }
                    }
                }));

        var sut = new JDBRoundDetailGetService(
            fakeLogger,
            fakeRoundService,
            fakePlayerService,
            fakeGameProviderService,
            fakeGameService,
            fakeService,
            fakeAgentMapper,
            fakeApiClient);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        CollectionAssert.AreEqual(expected.Data, actual.Data);
    }
}
